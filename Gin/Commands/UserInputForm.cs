using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Gin.Attributes;
using Gin.Context;
using Gin.Controls;
using System.Threading;
using System.Xml.Serialization;

namespace Gin.Commands
{
    delegate void AddControlDelegate(Control c);

    [GinName(Name = "Форма ввода", Description = "Отображает форму ввода пользовательских данных", Group = "Управление")]
    public class UserInputForm : Command, IReversibleCommand, IContainerCommand
    {

        #region Аргументы команды

        [GinArgumentText(Multiline = false, Name = "Заголовок формы", Description = "Заголовок формы ввода")]
        public string FormCaption { get; set; }

        [GinArgumentCommand(IsEnumerable = true, Description = "Поля ввода", Name = "Поля ввода")]
        [GinArgumentCommandAcceptOnly(AcceptedType = typeof(UserInputControl))]
        public List<UserInputControl> InputControls { get; set; }

        [GinArgumentCommand(IsEnumerable = false, Name = "После закрытия", Description = "Команды выполянемая после закрытия формы")]
        [GinArgumentCommandAcceptNot(NotAcceptedType = typeof(UserInputControl))]
        public Command AfterComplete { get; set; }

        [GinArgumentCommand(IsEnumerable = false, Name = "Перед открытием", Description = "Команда выполянемая перед открытием формы")]
        [GinArgumentCommandAcceptNot(NotAcceptedType = typeof(UserInputControl))]
        public Command BeforeCreate { get; set; }

        #endregion

        private readonly AutoResetEvent _nextWaitHandle = new AutoResetEvent(false);
        private IExecutionContext _context;

        private CommandResult _commandResult = CommandResult.Next;

        public UserInputForm()
        {
            _progressCost = 0;
        }

        public override CommandResult Do(IExecutionContext context)
        {
            _context = context;

            FlowLayoutPanel mainPanel = null;
            context.ControlContainer.Invoke(new Action(() =>
                {
                    context.ControlContainer.SuspendLayout();
                    context.ControlContainer.Controls.Clear();
                    context.ControlContainer.Text = FormCaption;
                    mainPanel = new FlowLayoutPanel {Dock = DockStyle.Fill};
                    context.ControlContainer.Controls.Add(mainPanel);
                }
            ));

            if (BeforeCreate != null)
            {
                BeforeCreate.Do(context);
            }

            CreateUserInputControls(mainPanel);
            CreateCommonControls();

            context.ControlContainer.Invoke(new Action<bool>(context.ControlContainer.ResumeLayout), true);

            _nextWaitHandle.WaitOne();

            if (AfterComplete != null && _commandResult == CommandResult.Next)
            {
                AfterComplete.Do(context);
            }

            context.ControlContainer.Invoke(new Action(context.ControlContainer.Controls.Clear), null);

            return _commandResult;
        }

        private void CreateUserInputControls(Control mainPanel)
        {

            foreach (UserInputControl input in InputControls)
            {
                Control control = input.Create(_context);
                control.Left = _context.ControlContainer.Padding.Left;
                control.Width = _context.ControlContainer.Width - (_context.ControlContainer.Padding.Left + _context.ControlContainer.Padding.Right);
                control.Tag = input;
                _context.ControlContainer.Invoke(new AddControlDelegate(mainPanel.Controls.Add), control);

            }
        }

        private void CreateCommonControls()
        {
            const int BUTTON_WIDTH = 80;
            const int BUTTON_PADDING = 20;
            const int BUTTON_TOP = 2;
            Panel panel = new Panel {Dock = DockStyle.Bottom, Height = 27};

            Button prevButton = new Button
                                    {
                                        Anchor = AnchorStyles.Right,
                                        Top = BUTTON_TOP,
                                        Left = 0,
                                        Text = "Назад...",
                                        Width = BUTTON_WIDTH
                                    };
            prevButton.Click += PrevButtonClick;
            prevButton.Enabled = !IsFirst;

            panel.Controls.Add(prevButton);

            Button nextButton = new Button
                                    {
                                        Anchor = AnchorStyles.Right,
                                        Top = BUTTON_TOP,
                                        Left = prevButton.Right + BUTTON_PADDING,
                                        Text = IsLast ? "Старт" : "Дальше...",
                                        Width = BUTTON_WIDTH
                                    };
            nextButton.Click += NextButtonClick;
            nextButton.Enabled = true;

            panel.Controls.Add(nextButton);

            _context.ControlContainer.Invoke(new AddControlDelegate(_context.ControlContainer.Controls.Add), panel);
       }

        void PrevButtonClick(object sender, EventArgs e)
        {
            _commandResult = CommandResult.Previous;
            _nextWaitHandle.Set();
        }

        void NextButtonClick(object sender, EventArgs e)
        {
            _commandResult = CommandResult.Next;
            ReadValueFromUserInputs();
            _nextWaitHandle.Set();
        }

        private void ReadValueFromUserInputs()
        {
            foreach (UserInputControl control in InputControls)
            {
                SaveControlValueToContext(control);
            }
        }

        private void SaveControlValueToContext(UserInputControl input)
        {
            object value = input.Value;
            _context.SaveResult(input.ResultName, value, true);
        }

        public bool IsFirst { get; set; }

        public bool IsLast { get; set; }

        [XmlIgnore]
        public IEnumerable<Command> InnerCommands
        {
            get 
            {
                List<Command> list = new List<Command>();
                if (AfterComplete != null)
                {
                    list.Add(AfterComplete);
                }
                if (BeforeCreate != null)
                {
                    list.Add(BeforeCreate);
                }
                return list;
            }
        }

        public override void Visit(CommandVisitor visitor)
        {
            base.Visit(visitor);
            if (AfterComplete != null)
            {
                AfterComplete.Visit(visitor);
            }
            if (BeforeCreate != null)
            {
                BeforeCreate.Visit(visitor);
            }
            if (InputControls != null)
            {
                foreach (var item in InputControls)
                {
                    item.Visit(visitor);
                }
            }
        }

    }
}

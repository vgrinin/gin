using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using Gin;
using Gin.Util;
using Gin.Commands;
using Gin.Controls;
using Gin.Logging;
using System.Configuration;
using Gin.SQL.Util;
using Gin.SQL.Commands;
using Gin.SQL.Controls;
using Avicomp.Installer;
using Gin.SQL;


namespace Gin.Installer
{
    public static class PackageCreator
    {

        private const string CONNECTION_STRING1 = @"Data Source=.;Initial Catalog=GinDB;Persist Security Info=True;User ID=sa;Password=osbb";
        private const string CONNECTION_STRING_TEMPLATE = @"Data Source=%server_name%;Initial Catalog=%db_name%;Persist Security Info=True;User ID=%user_name%;Password=%user_password%";

        #region Тестовые пакеты

        public static PackageBody CreatePackage1()
        {
            PackageBody body = new PackageBody();
            body.Command = new CommandSequence()
            {
                Commands = new List<Command>()
                {
                    new SaveDateTime()
                    {
                         Value = DateTime.Now,
                         ResultName = "dtnow"
                    },
                    new FormatDateTime()
                    {
                         DateTimeValue = DateTime.Now,
                         FormatString = "ddMMyyyy",
                         ResultName = "formatteddate",
                         UseCurrent = true
                    },
                    new CMExecuteSQLQuery()
                    {
                         CommandText = "select * from t1",
                         CommandTimeout = 201,
                         CommandType = CommandType.Text,
                         ConnectionString = CONNECTION_STRING1,
                         ParameterNames = new List<SqlParameterName>()
                         {
                             new SqlParameterName()
                             {
                                  ParameterName = "par1"
                             },
                             new SqlParameterName()
                             {
                                  ParameterName = "par2"
                             }
                         },
                         ResultName = "sql_result"               
                    },
                    new CreateTextFile()
                    {
                         SourceFilePath = @"C:\_test\conn.txt",
                         DestFilePath = @"C:\_test\conn2.txt",
                         Substitutes = new List<StringSubstitute>()
                         {
                             new StringSubstitute()
                             {
                                  Key = "{SERVER}",
                                  Value = "sdb14"
                             },
                             new StringSubstitute()
                             {
                                 Key = "{BASE}",
                                 Value = "GRBS_437_RAP"
                             }
                         }
                    }
                }
            };
            return body;
        }

        public static PackageBody CreatePackage2()
        {
            PackageBody body = new PackageBody();
            body.Command = new TransactionContainer()
            {
                TransactionName = "create_backup",
                Command = new CommandSequence()
                {
                    Commands = new List<Command>()
                    {
                        new UserInputForm()
                        {
                            FormCaption = "Подключение к БД",
                            InputControls = new List<UserInputControl>()
                            {
                               new ICSQLConnection()
                               {
                                   InitialValue = new SQLConnectionProperties()
                                   {
                                        DBName = "GRBS_437",
                                        InstanceName = ".",
                                        Password = "osbb",
                                        SqlAuthentication = true,
                                        UserName = "sa"
                                   },
                                   ResultName = "sql_connection_string"
                               }
                            },
                            AfterComplete = new CMParseResult()
                            {
                                ArgumentName = "%sql_connection_string%"
                            }
                        },
                        new UserInputForm()
                        {
                             FormCaption = "Куда сохранять",
                             InputControls = new List<UserInputControl>()
                             {
                                 new UserInputBrowseDirectoryDialog()
                                 {
                                      Caption = "Куда сохраняем",
                                      InitialValue = @"C:\_test\",
                                      ResultName = "backup_path"
                                 }
                             }
                        },
                        new FormatDateTime()
                        {
                             UseCurrent = true,
                             FormatString = "yyyyMMddHHmmss",
                             ResultName = "date_time_string"
                        },
                        new SaveString()
                        {
                             IsPathCombine = true,
                             Value = @"%backup_path%\%date_time_string%_%sql_connection_string.DBName%.bak",
                             ResultName = "backup_full_path"
                        },
                        new CMCreateSQLBackup()
                        {
                             BackupFilePath = "%backup_full_path%",
                             CommandTimeout = 3600,
                             ConnectionString = "%sql_connection_string.ConnectionString%",
                             DatabaseName = "%sql_connection_string.DBName%"
                        }
                    }
                }
            };

            return body;
        }

        public static PackageBody CreatePackage3()
        {
            PackageBody body = new PackageBody();
            body.PackageName = "Офигенский тестовый пакет";
            body.Command = new CommandSequence()
            {
                Commands = new List<Command>()
                {
                    new UserInputForm()
                    {
                        BeforeCreate = new CMExecuteSQLQuery()
                        {
                             CommandText = "select * from [values]",
                             CommandType = CommandType.Text,
                             ConnectionString = CONNECTION_STRING1,
                             ResultName = "sql_list"
                        },
                        FormCaption = "Тестируем контролы",
                        InputControls = new List<UserInputControl>()
                        {
                            new UserInputComboBox()
                            {
                                Caption = "Вы уверены?",
                                ResultName = "listbox_1",
                                ListDataName = "%sql_list%",
                                DropDownOnly = true,
                                DisplayMember = "val",
                                ValueMember = "id"
                            }
                        },
                         AfterComplete = new ExtractDataRowField()
                         {
                              DataRowName = "%listbox_1%",
                              FieldName = "id",
                              ResultName ="listbox_1_value" 
                         }
                    },
                    new UserInputForm()
                    {
                        FormCaption = "Еще форма",
                        InputControls = new List<UserInputControl>()
                        {
                            new UserInputTextBox()
                            {
                                Caption = "вот что вы выбрали",
                                InitialValue = "%listbox_1_value%",
                                ResultName = "textbox_2"
                            }
                        }
                    }
                }
            };

            return body;
        }

        public static PackageBody CreatePackage4()
        {
            PackageBody body = new PackageBody();
            body.Command = new CommandSequence()
            {
                Commands = new List<Command>()
                {
                    new UserInputForm()
                    {
                        FormCaption = "Тестируем контролы",
                        InputControls = new List<UserInputControl>()
                        {
                            new ICSQLConnection()
                            {
                                 ResultName = "sql_server_cs"
                            }
                        },
                        AfterComplete = new CMExecuteSQLQuery()
                        {
                            CommandText = "select * from [values]",
                            CommandType = CommandType.Text,
                            ConnectionString = "%sql_server_cs%",
                            ResultName = "sql_list"
                        }
                    },
                    new UserInputForm()
                    {
                        FormCaption = "Тестируем контролы",
                        InputControls = new List<UserInputControl>()
                        {
                            new UserInputComboBox()
                            {
                                Caption = "Вот оно",
                                ResultName = "listbox_1",
                                ListDataName = "%sql_list%",
                                DropDownOnly = true,
                                DisplayMember = "val",
                                ValueMember = "id"
                            }
                        }
                    }
                }
            };

            return body;
        }

        public static PackageBody CreatePackage5()
        {
            PackageBody body = new PackageBody();
            body.Command = new CommandSequence()
            {
                Commands = new List<Command>()
                {
                    new CreateDirectoryContent()
                    {
                         DestPath = @"C:\_test\proj\",
                         SourcePath = @"D:\work\_projects\Gin.Installer\gin-installer\AuxPackages\TestPackage\"
                    }
                }
            };

            return body;
        }

        public static PackageBody CreatePackage6()
        {
            PackageBody body = new PackageBody();
            body.Command = new CommandSequence()
            {
                Commands = new List<Command>()
                {
                    new CreateDirectoryContent()
                    {
                         DestPath = @"%my_dir%",
                         SourcePath = @"D:\install\TotalCmd 6.0\"
                    },
                    new CreateFile()
                    {
                         SourcePath = @"D:\install\berkleydb\db-5.2.36.msi",
                         DestPath = @"%my_dir%\db-5.2.36.msi"
                    },
                    new ExecuteProgram()
                    { 
                        IntResultName = "bdb_res",
                        ProgramExePath = @"%my_dir%\db-5.2.36.msi",
                        WindowType = Gin.Util.ProgramWindowType.WinForms
                    },
                    new ExecuteProgram()
                    { 
                        IntResultName = "tc_res",
                        ProgramExePath = @"%my_dir%\Total Commander 6.0 Russian.exe",
                        WindowType = Gin.Util.ProgramWindowType.WinForms
                    }
                }
            };

            return body;
        }

        public static PackageBody CreatePackage7()
        {
            PackageBody body = new PackageBody();
            body.Command = new CommandSequence()
            {
                Commands = new List<Command>()
                {
                    new IsCOMInstalled()
                    {
                         ClassId = "{78BEC353-56DD-4152-AB5D-A4BE89DB4160}",
                         ResultName = "isInstalled"
                    }
                }
            };

            return body;
        }

        public static PackageBody CreatePackage8()
        {
            PackageBody body = new PackageBody();
            body.Command = new CommandSequence()
            {
                Commands = new List<Command>()
                {
                   /* new IsMSIInstalled()
                    {
                        SearchType = MSISearchType.ProductName,
                        Product = "Microsoft SQL Server 2005",
                        ResultName = "sqlexpr2005IsInstalled"
                    },
                    new ExecuteIf()
                    {
                        ArgumentName = "%sqlexpr2005IsInstalled%",
                        Else = new CommandSequence()
                        {
                            Commands = new List<Command>()
                            {
                                new CreateFile()
                                {
                                    SourcePath = @"D:\install\DTS\SQLEXPR.EXE",
                                    DestPath = @"%PACKAGE_CONTENT%\SQLEXPR.EXE"
                                },
                                new ExecuteProgram()
                                {
                                    ProgramExePath = @"%PACKAGE_CONTENT%\SQLEXPR.EXE",
                                    WindowType = Gin.Util.ProgramWindowType.WinForms,
                                    IntResultName = "sqlexprInstallExitCode"
                                }
                            }
                        }
                    },
                  */
                    
                    new IsMSIInstalled()
                    {
                        SearchType = MSISearchType.ProductName,
                        Product = "SQL Server 2000 DTS Designer Components",
                        ResultName = "dts2005IsInstalled"
                    },
                    new ExecuteIf()
                    {
                        ArgumentName = "%dts2005IsInstalled%",
                        Then = new ShowMessage()
                        {
                             MessageText = "DTS Designer Components был установлен ранее"
                        },
                        Else = new CommandSequence()
                        {
                            Commands = new List<Command>()
                            {
                                new CreateFile()
                                {
                                    SourcePath = @"D:\install\DTS\SQLServer2005_DTS.msi",
                                    DestPath = @"%PACKAGE_CONTENT%\SQLServer2005_DTS.msi"
                                },
                                new ExecuteProgram()
                                {
                                     ProgramExePath = @"%PACKAGE_CONTENT%\SQLServer2005_DTS.msi",
                                     WindowType = Gin.Util.ProgramWindowType.WinForms,
                                     IntResultName = "dtsInstallExitCode"
                                }
                            }
                        }
                    },

                    /*
                    new IsMSIInstalled()
                    {
                        SearchType = MSISearchType.ProductName,
                        Product = "Microsoft SQL Server 2005 Backward compatibility",
                        ResultName = "bc2005IsInstalled"
                    },
                    new ExecuteIf()
                    {
                        ArgumentName = "%bc2005IsInstalled%",
                        Else = new CommandSequence()
                        {
                            Commands = new List<Command>()
                            {
                                new CreateFile()
                                {
                                     SourcePath = @"D:\install\DTS\SQLServer2005_BC.msi",
                                     DestPath = @"%PACKAGE_CONTENT%\SQLServer2005_BC.msi"
                                },
                                new ExecuteProgram()
                                {
                                     ProgramExePath = @"%PACKAGE_CONTENT%\SQLServer2005_BC.msi",
                                     WindowType = Gin.Util.ProgramWindowType.WinForms,
                                     IntResultName = "bcInstallExitCode"
                                }
                            }
                        }
                    }*/
                }
            };

            return body;
        }

        public static PackageBody CreatePackage11()
        {
            PackageBody body = new PackageBody();
            body.Command = new CommandSequence()
            {
                Commands = new List<Command>()
                {
                    new CMInstallSQLInstance()
                    {
                         ResultName = "sqlInstallResult",
                         SAPassword = "osbb45ES%",
                         InstanceName = "OSBB",
                         SetupFilePath = @"D:\work\minfin\Setup\SetupOSBB\INSTALL\SQLEXPR_RUS.EXE"
                    }
                }
            };

            return body;
        }


        #endregion

        #region Основные пакеты

        /// <summary>
        /// Пакет инсталлятора
        /// </summary>
        public static PackageBody CreatePackageInstaller()
        {
            PackageBody body = new PackageBody();
            body.Height = 250;
            body.Width = 500;
            body.Command = new CommandSequence()
            {
                Commands = new List<Command>()
                {
                    new UserInputForm()
                    {
                        FormCaption = "Папка установки ППО Сводная отчетность",
                        BeforeCreate = new CommandSequence()
                        {
                            Commands = new List<Command>()
                            {
                                new ReadEnvironment()
                                {
                                     VariableName = "PROGRAMFILES",
                                     ResultName = "program_files"
                                },
                                new SaveString()
                                {
                                    Value = @"%program_files%\Avicomp\OSBB\",
                                    ResultName = "osbb_folder_init"
                                }
                            }
                        },
                        InputControls = new List<UserInputControl>()
                        {
                            new UserInputBrowseDirectoryDialog()
                            {
                                Caption = "Выберите папку установки ППО Сводная отчетность",
                                ResultName = "osbb_install_folder",
                                InitialValue = @"%osbb_folder_init%"
                            },
                            new UserInputCheckBox()
                            { 
                                Caption = "Создать ярлык на рабочем столе",
                                InitialValue = true, 
                                ResultName = "create_desktop_shortcut"
                            },
                            new UserInputCheckBox()
                            { 
                                Caption = "Создать ярлык в меню Пуск",
                                InitialValue = false, 
                                ResultName = "create_start_shortcut"
                            },
                            new UserInputCheckBox()
                            { 
                                Caption = "Сделать ярлыки доступными всем пользователям",
                                InitialValue = true, 
                                ResultName = "create_allusers_shortcut"
                            }
                        }
                    },
                    new UserInputForm()
                    {
                         FormCaption = "Выбор экземпляра SQL сервера",
                         BeforeCreate = new CommandSequence()
                         {
                             Commands = new List<Command>()
                             {
                                 new ReadEnvironment()
                                 {
                                    VariableName = "SYSTEMDRIVE",
                                    ResultName = "system_drive"
                                 },
                                 new SaveString()
                                 {
                                    Value = @"%system_drive%\OSBB_SQL\",
                                    ResultName = "data_sql_folder"
                                 }
                             }
                         },
                         InputControls = new List<UserInputControl>()
                         {
                            new ICChooseSQLInstance()
                            {
                                 ResultName = "choose_sql_instance", 
                                 InitialValue = new ICSQLInstance()
                                 {
                                      InstallNewInstance = true,
                                      InstanceName = @".\OSBB",
                                      SqlDataDirectory = @"%data_sql_folder%"
                                 }
                            }
                         },
                         AfterComplete = new CommandSequence()
                         {
                             Commands = new List<Command>()
                             {
                                 new CMParseResult()
                                 {
                                     ArgumentName = "%choose_sql_instance%"
                                 }
                             }
                         }
                    },

                    new UserInputForm()
                    {
                         InputControls = new List<UserInputControl>()
                         {
                              new ICSQLConnection()
                              {
                                    ResultName = "sql_connection",
                                    FixedInstanceName = true,
                                    FixedUserName = "%choose_sql_instance.InstallNewInstance%",
                                    FixedSqlAuth = "%choose_sql_instance.InstallNewInstance%",
                                    FixedDBName = false,
                                    InitialValue = new SQLConnectionProperties()
                                    {
                                        InstanceName = "%choose_sql_instance.InstanceName%",
                                        DBName = "OSBB",
                                        SqlAuthentication = true,
                                        UserName = "sa",
                                        Password = ""
                                    }
                              }
                         },
                         FormCaption = "Параметры подключения",
                         AfterComplete = new CommandSequence()
                         {
                             Commands = new List<Command>()
                             {
                                 new CMParseResult()
                                 {
                                     ArgumentName = "%sql_connection%"
                                 }
                             }
                         }
                    }/*,

                    new CommandSequence()
                    {
                        Commands = new List<Command>()
                        {
                            new ExecuteIf()
                            {
                                ArgumentName = "%choose_sql_instance.InstallNewInstance%",
                                Then = new CMInstallSQLInstance()
                                {
                                    SetupFilePath = @"D:\install\msoe2007kg.exe",
                                    InstanceName = "%choose_sql_instance.InstanceName%"
                                }
                            },
                            new CMRestoreSQLBackup()
                            {
                                BackupFilePath = @"C:\_test\_GinDB.bak",
                                ConnectionString = "%sql_connection.ConnectionString%", 
                                DatabaseName = "%sql_connection.DBName%"
                            },
                            new CreateDirectoryContent()
                            {
                                 SourcePath = @"C:\_test\osbb\",
                                 DestPath = @"%osbb_install_folder%"
                            },
                            new CreateTextFile()
                            {
                                 SourceFilePath = @"%osbb_install_folder%\options.ini",
                                 DestFilePath = @"%osbb_install_folder%\options.ini",
                                 Substitutes = new List<StringSubstitute>()
                                 {
                                     new StringSubstitute()
                                     {
                                          Key = "SERVER_NAME",
                                          Value = "%sql_connection.InstanceName%"
                                     },
                                     new StringSubstitute()
                                     {
                                          Key = "DB_NAME",
                                          Value = "%sql_connection.DBName%"
                                     },
                                 }
                            },
                            new ExecuteIf()
                            {
                                 ArgumentName = "%create_desktop_shortcut%",
                                 Then = new CreateShortcut()
                                 {
                                     AllUsers = "%create_allusers_shortcut%",
                                     Place = ShortcutPlace.Desktop,
                                     ShortcutName = "Сводная отчетность",
                                     FilePath = @"%osbb_install_folder%\SvodOsbb.exe"
                                 }
                            },
                            new ExecuteIf()
                            {
                                 ArgumentName = "%create_start_shortcut%",
                                 Then = new CreateShortcut()
                                 {
                                     AllUsers = "%create_allusers_shortcut%",
                                     Place = ShortcutPlace.StartMenu,
                                     ShortcutName = "Сводная отчетность",
                                     FilePath = @"%osbb_install_folder%\SvodOsbb.exe"
                                 }
                            }
                        }
                    }*/
                }
            };

            return body;
        }

        /// <summary>
        /// Пакет апдейтера
        /// </summary>
        public static PackageBody CreatePackageUpdater436()
        {
            PackageBody body = new PackageBody();
            body.Command = new CommandSequence()
            {
                Commands = new List<Command>() 
                {
                    new UserInputForm()
                    {
                         InputControls= new List<UserInputControl>()
                         {
                             new ICSQLConnection()
                             {
                                  InitialValue= new SQLConnectionProperties()
                                  {
                                       DBName = "GRBS_186",
                                       InstanceName = ".",
                                       UserName = "sa",
                                       SqlAuthentication = true,
                                       Password = "osbb"
                                  },
                                  ResultName = "sql_connection"
                             }
                         },
                         FormCaption = "Подключение к БД",
                         AfterComplete = new CMParseResult()
                         {
                             ArgumentName = "%sql_connection%"
                         }
                    },
                    new UserInputForm()
                    {
                        BeforeCreate = new CommandSequence()
                        {
                            Commands = new List<Command>()
                            {
                                new ReadEnvironment()
                                {
                                    VariableName = "SYSTEMDRIVE",
                                    ResultName = "system_drive"
                                },
                                new FormatDateTime()
                                {
                                     FormatString = "yyyyMMdd",
                                     UseCurrent = true,
                                     ResultName = "date_time_string"
                                },
                                new SaveString()
                                {
                                    Value = "%sql_connection.DBName%_%date_time_string%_1.bak",
                                    ResultName = "backup_filename_before"
                                },
                                new SaveString()
                                {
                                    Value = "%sql_connection.DBName%_%date_time_string%_2.bak",
                                    ResultName = "backup_filename_after"
                                }
                            }
                        },
                        InputControls = new List<UserInputControl>()
                        {
                            new UserInputBrowseDirectoryDialog()
                            {
                                Caption = "Папка для сохранения бэкапов",
                                InitialValue = "%system_drive%",
                                ResultName = "backups_folder"
                            },
                            new UserInputCheckBox()
                            {
                                 Caption = "Будем бэкапить до выполнения?",
                                 InitialValue = true,
                                 ResultName = "need_backup_before"
                            },
                            new UserInputTextBox()
                            {
                                 Caption = "Имя бэкапа до выполнения",
                                 InitialValue = "%backup_filename_before%",
                                 ResultName = "backup_filename_before"
                            },
                            new UserInputCheckBox()
                            {
                                 Caption = "Будем бэкапить после выполнения?",
                                 InitialValue = true,
                                 ResultName = "need_backup_after"
                            },
                            new UserInputTextBox()
                            {
                                 Caption = "Имя бэкапа после выполнения",
                                 InitialValue = "%backup_filename_after%",
                                 ResultName = "backup_filename_after"
                            }
                       },
                       AfterComplete = new CommandSequence()
                       {
                            Commands = new List<Command>()
                            {
                                new SaveString()
                                {
                                    Value = @"%backups_folder%\%backup_filename_before%",
                                    ResultName = "backup_full_filename_before",
                                    IsPathCombine = true
                                },
                                new SaveString()
                                {
                                    Value = @"%backups_folder%\%backup_filename_after%",
                                    ResultName = "backup_full_filename_after",
                                    IsPathCombine = true
                                }
                            }
                        }
                    },
                   new CommandSequence()
                    {
                        Commands = new List<Command>()
                        {
                            new CreateDirectoryContent()
                            {
                                SourcePath = @"D:\20120312_UPDATE_436_new\included_\58a2a46323c8431d867460a47a196061.cnt\",
                                DestPath = @"%PACKAGE_CONTENT%\SCRIPTS",
                                MoveFast = true
                            },
                            new CreateDirectoryContent()
                            {
                                SourcePath = @"D:\20120312_UPDATE_436_new\included_\c5dc8301e71a4d6eba5cca089cd97376.cnt\",
                                DestPath = @"%PACKAGE_CONTENT%\FILES",
                                MoveFast = true
                            },
                            new CreateDirectoryContent()
                            {
                                 SourcePath = @"D:\20120312_UPDATE_436_new\included_\f018a57096354cb889d261bd3274dab2.cnt\",
                                 DestPath = @"%PACKAGE_CONTENT%\DTS",
                                 MoveFast = true
                            },
                            new ExecuteIf()
                            {
                                 ArgumentName = "%need_backup_before%",
                                 Then = new CMCreateSQLBackup()
                                 {
                                     ConnectionString = "%sql_connection.ConnectionString%",
                                     CommandTimeout = 3600,
                                     DatabaseName = "%sql_connection.DBName%",
                                     BackupFilePath = "%backup_full_filename_before%"
                                 }
                            },
                            new CMSynchronize()
                            {
                                 ConnectionString = "%sql_connection.ConnectionString%",
                                 ConnectionStringEtalon = "",
                                 DTSDirectoryPath = @"%PACKAGE_CONTENT%\DTS",
                                 SyncType = OSBBSynchronizationType.TextFile,
                                 CheckFormulaDiapasons = true, 
                                 LoadHierarchy = false,
                                 UpdateSetStrings = false,
                                 PackagePath = @"%PACKAGE_CONTENT%\FILES\29", 
                                 LoadPick = false, 
                                 UpdateDisplayCode = false
                            },
                            new CMExecuteSQLNonQuery()
                            {
                                 ScriptFilePath = @"%PACKAGE_CONTENT%\SCRIPTS\1_UPD_StringIndex.sql",
                                 CommandType = System.Data.CommandType.Text,
                                 ConnectionString = "%sql_connection.ConnectionString%"       
                            },
                            new CMExecuteSQLNonQuery()
                            {
                                 ScriptFilePath = @"%PACKAGE_CONTENT%\SCRIPTS\2_Update_OuterConrol.sql",
                                 CommandType = System.Data.CommandType.Text,
                                 ConnectionString = "%sql_connection.ConnectionString%"       
                            },
                            new CMExecuteSQLNonQuery()
                            {
                                 ScriptFilePath = @"%PACKAGE_CONTENT%\SCRIPTS\3_UpdateModelFilters.sql",
                                 CommandType = System.Data.CommandType.Text,
                                 ConnectionString = "%sql_connection.ConnectionString%"       
                            },
                            new CMExecuteSQLNonQuery()
                            {
                                 ScriptFilePath = @"%PACKAGE_CONTENT%\SCRIPTS\4_BuildFormulas.sql",
                                 CommandType = System.Data.CommandType.Text,
                                 ConnectionString = "%sql_connection.ConnectionString%"       
                            },
                            new ExecuteIf()
                            {
                                 ArgumentName = "%need_backup_after%",
                                 Then = new CMCreateSQLBackup()
                                 {
                                     ConnectionString = "%sql_connection.ConnectionString%",
                                     CommandTimeout = 3600,
                                     DatabaseName = "%sql_connection.DBName%",
                                     BackupFilePath = "%backup_full_filename_after%"
                                 }
                            },

#region commented
                            /*
                            new CMImportSets()
                            {
                                ExcelFilePath = @"%PACKAGE_CONTENT%\EXCEL\БР_ДОХОДЫ_год.xls",
                                WorksheetName = "1",
                                StartRowNumber = 5,
                                EndRowNumber = 2873,
                                ConnectionString = @"%sql_connection.ConnectionString%",
                                DTSFilePath = @"%PACKAGE_CONTENT%\DTS\rp_ImportExcel.dts",
                                IgnoreDuplicates = false,
                                DeletePreviousSets = true,
                                NSIVersion = 2,
                                Secret = SecretType.NoSecret,
                                SetName = "БР_ДОХОДЫ_год"
                            },
                             new CMImportSets()
                            {
                                ExcelFilePath = @"%PACKAGE_CONTENT%\EXCEL\БР_ДОХОДЫ.xls",
                                WorksheetName = "1",
                                StartRowNumber = 5,
                                EndRowNumber = 3663,
                                ConnectionString = @"%sql_connection.ConnectionString%",
                                DTSFilePath = @"%PACKAGE_CONTENT%\DTS\rp_ImportExcel.dts",
                                IgnoreDuplicates = false,
                                DeletePreviousSets = true,
                                NSIVersion = 2,
                                Secret = SecretType.NoSecret,
                                SetName = "БР_ДОХОДЫ"
                            },
                            new CMImportSets()
                            {
                                ExcelFilePath = @"%PACKAGE_CONTENT%\EXCEL\БР_ИФД.xls",
                                WorksheetName = "1",
                                StartRowNumber = 5,
                                EndRowNumber = 528,
                                ConnectionString = @"%sql_connection.ConnectionString%",
                                DTSFilePath = @"%PACKAGE_CONTENT%\DTS\rp_ImportExcel.dts",
                                IgnoreDuplicates = false,
                                DeletePreviousSets = true,
                                NSIVersion = 2,
                                Secret = SecretType.NoSecret,
                                SetName = "БР_ИФД"
                            },
                            new CMImportSets()
                            {
                                ExcelFilePath = @"%PACKAGE_CONTENT%\EXCEL\БР_ИФД_год.xls",
                                WorksheetName = "1",
                                StartRowNumber = 5,
                                EndRowNumber = 540,
                                ConnectionString = @"%sql_connection.ConnectionString%",
                                DTSFilePath = @"%PACKAGE_CONTENT%\DTS\rp_ImportExcel.dts",
                                IgnoreDuplicates = false,
                                DeletePreviousSets = true,
                                NSIVersion = 2,
                                Secret = SecretType.NoSecret,
                                SetName = "БР_ИФД_год"
                            },
                            new CMImportSets()
                            {
                                ExcelFilePath = @"%PACKAGE_CONTENT%\EXCEL\БР_РАСХОДЫ.xls",
                                WorksheetName = "1",
                                StartRowNumber = 5,
                                EndRowNumber = 4182,
                                ConnectionString = @"%sql_connection.ConnectionString%",
                                DTSFilePath = @"%PACKAGE_CONTENT%\DTS\rp_ImportExcel.dts",
                                IgnoreDuplicates = false,
                                DeletePreviousSets = true,
                                NSIVersion = 2,
                                Secret = SecretType.NoSecret,
                                SetName = "БР_РАСХОДЫ"
                            },
                            new CMImportSets()
                            {
                                ExcelFilePath = @"%PACKAGE_CONTENT%\EXCEL\БР_РАСХОДЫ_год.xls",
                                WorksheetName = "1",
                                StartRowNumber = 5,
                                EndRowNumber = 4182,
                                ConnectionString = @"%sql_connection.ConnectionString%",
                                DTSFilePath = @"%PACKAGE_CONTENT%\DTS\rp_ImportExcel.dts",
                                IgnoreDuplicates = false,
                                DeletePreviousSets = true,
                                NSIVersion = 2,
                                Secret = SecretType.NoSecret,
                                SetName = "БР_РАСХОДЫ_год"
                            },
                            new CMImportSets()
                            {
                                ExcelFilePath = @"%PACKAGE_CONTENT%\EXCEL\БР_ДОХОДЫ_210_2011.xls",
                                WorksheetName = "1",
                                StartRowNumber = 5,
                                EndRowNumber = 58,
                                ConnectionString = @"%sql_connection.ConnectionString%",
                                DTSFilePath = @"%PACKAGE_CONTENT%\DTS\rp_ImportExcel.dts",
                                IgnoreDuplicates = false,
                                DeletePreviousSets = true,
                                NSIVersion = 2,
                                Secret = SecretType.NoSecret,
                                SetName = "БР_ДОХОДЫ_210_2011"
                            },
                            new CMImportSets()
                            {
                                ExcelFilePath = @"%PACKAGE_CONTENT%\EXCEL\БР_ДОХОДЫ_284_2011.xls",
                                WorksheetName = "1",
                                StartRowNumber = 5,
                                EndRowNumber = 69,
                                ConnectionString = @"%sql_connection.ConnectionString%",
                                DTSFilePath = @"%PACKAGE_CONTENT%\DTS\rp_ImportExcel.dts",
                                IgnoreDuplicates = false,
                                DeletePreviousSets = true,
                                NSIVersion = 2,
                                Secret = SecretType.NoSecret,
                                SetName = "БР_ДОХОДЫ_284_2011"
                            },
                            new CMImportSets()
                            {
                                ExcelFilePath = @"%PACKAGE_CONTENT%\EXCEL\БР_ИФД_284_2011.xls",
                                WorksheetName = "1",
                                StartRowNumber = 5,
                                EndRowNumber = 70,
                                ConnectionString = @"%sql_connection.ConnectionString%",
                                DTSFilePath = @"%PACKAGE_CONTENT%\DTS\rp_ImportExcel.dts",
                                IgnoreDuplicates = false,
                                DeletePreviousSets = true,
                                NSIVersion = 2,
                                Secret = SecretType.NoSecret,
                                SetName = "БР_ИФД_284_2011"
                            },
                            new CMImportSets()
                            {
                                ExcelFilePath = @"%PACKAGE_CONTENT%\EXCEL\БР_ДОХОДЫ_284_2012.xls",
                                WorksheetName = "1",
                                StartRowNumber = 5,
                                EndRowNumber = 67,
                                ConnectionString = @"%sql_connection.ConnectionString%",
                                DTSFilePath = @"%PACKAGE_CONTENT%\DTS\rp_ImportExcel.dts",
                                IgnoreDuplicates = false,
                                DeletePreviousSets = true,
                                NSIVersion = 2,
                                Secret = SecretType.NoSecret,
                                SetName = "БР_ДОХОДЫ_284_2012"
                            },
                            new CMImportSets()
                            {
                                ExcelFilePath = @"%PACKAGE_CONTENT%\EXCEL\БР_ИФД_284_2012.xls",
                                WorksheetName = "1",
                                StartRowNumber = 5,
                                EndRowNumber = 59,
                                ConnectionString = @"%sql_connection.ConnectionString%",
                                DTSFilePath = @"%PACKAGE_CONTENT%\DTS\rp_ImportExcel.dts",
                                IgnoreDuplicates = false,
                                DeletePreviousSets = true,
                                NSIVersion = 2,
                                Secret = SecretType.NoSecret,
                                SetName = "БР_ИФД_284_2012"
                            },*/
#endregion

                       }
                    }
                }
            };

            return body;
        }

        /// <summary>
        /// Пакет апдейтера
        /// </summary>
        public static PackageBody CreatePackageUpdaterRAP()
        {
            PackageBody body = new PackageBody();
            body.PackageName = "Обновление 20120301_UPDATE_RAP";
            body.Command = new CommandSequence()
            {
                Commands = new List<Command>() 
                {
                    new UserInputForm()
                    {
                         InputControls= new List<UserInputControl>()
                         {
                             new ICSQLConnection()
                             {
                                  InitialValue= new SQLConnectionProperties()
                                  {
                                       DBName = "RAP",
                                       InstanceName = "ssun111",
                                       UserName = "sa",
                                       SqlAuthentication = true,
                                       Password = ""
                                  },
                                  ResultName = "sql_connection"
                             }
                         },
                         FormCaption = "Подключение к БД",
                         AfterComplete = new CMParseResult()
                         {
                             ArgumentName = "%sql_connection%"
                         }
                    },
                    new UserInputForm()
                    {
                        BeforeCreate = new CommandSequence()
                        {
                            Commands = new List<Command>()
                            {
                                new ReadEnvironment()
                                {
                                    VariableName = "SYSTEMDRIVE",
                                    ResultName = "system_drive"
                                },
                                new FormatDateTime()
                                {
                                     FormatString = "yyyyMMdd",
                                     UseCurrent = true,
                                     ResultName = "date_time_string"
                                },
                                new SaveString()
                                {
                                    Value = "%sql_connection.DBName%_%date_time_string%.bak",
                                    ResultName = "backup_filename"
                                },
                                new SaveString()
                                {
                                    Value = @"%system_drive%\%backup_filename%",
                                    ResultName = "backup_full_filename"
                                }
                            }
                        },
                        InputControls = new List<UserInputControl>()
                        {
                            new UserInputSaveFileDialog()
                            {
                                Caption = "Папка для сохранения бэкапа",
                                InitialValue = "%backup_full_filename%",
                                ResultName = "backup_full_filename"
                            },
                            new UserInputCheckBox()
                            {
                                 Caption = "Будем бэкапить?",
                                 InitialValue = true,
                                 ResultName = "need_backup"
                            }
                        }
                    },
                   new CommandSequence()
                    {
                        Commands = new List<Command>()
                        {
                            new CreateDirectoryContent()
                            {
                                SourcePath = @"D:\20120301_UPDATE_RAP\SCRIPTS\",
                                DestPath = @"%PACKAGE_CONTENT%\SCRIPTS",
                                MoveFast = true
                            },
                            new CreateDirectoryContent()
                            {
                                SourcePath = @"D:\20120301_UPDATE_RAP\FILES\",
                                DestPath = @"%PACKAGE_CONTENT%\FILES",
                                MoveFast = true
                            },
                            new CreateDirectoryContent()
                            {
                                 SourcePath = @"D:\work\_projects\Gin.Installer\gin-installer\AuxPackages\DTS\",
                                 DestPath = @"%PACKAGE_CONTENT%\DTS",
                                 MoveFast = true
                            },
                            new ExecuteIf()
                            {
                                 ArgumentName = "%need_backup%",
                                 Then = new CMCreateSQLBackup()
                                 {
                                     ConnectionString = "%sql_connection.ConnectionString%",
                                     CommandTimeout = 3600,
                                     DatabaseName = "%sql_connection.DBName%",
                                     BackupFilePath = "%backup_full_filename%"
                                 }
                            },
                            new CMSynchronize()
                            {
                                 ConnectionString = "%sql_connection.ConnectionString%",
                                 ConnectionStringEtalon = "",
                                 DTSDirectoryPath = @"%PACKAGE_CONTENT%\DTS",
                                 SyncType = OSBBSynchronizationType.TextFile,
                                 CheckFormulaDiapasons = true, 
                                 LoadHierarchy = false,
                                 UpdateSetStrings = false,
                                 PackagePath = @"%PACKAGE_CONTENT%\FILES\29", 
                                 LoadPick = false, 
                                 UpdateDisplayCode = false
                            },
                            new CMSynchronize()
                            {
                                 ConnectionString = "%sql_connection.ConnectionString%",
                                 ConnectionStringEtalon = "",
                                 DTSDirectoryPath = @"%PACKAGE_CONTENT%\DTS",
                                 SyncType = OSBBSynchronizationType.TextFile,
                                 CheckFormulaDiapasons = true, 
                                 LoadHierarchy = false,
                                 UpdateSetStrings = false,
                                 PackagePath = @"%PACKAGE_CONTENT%\FILES\30",
                                 LoadPick = false, 
                                 UpdateDisplayCode = false
                            },
                           new CMExecuteSQLNonQuery()
                            {
                                 ScriptFilePath = @"%PACKAGE_CONTENT%\SCRIPTS\1_UPD_StringIndex.sql",
                                 CommandType = System.Data.CommandType.Text,
                                 ConnectionString = "%sql_connection.ConnectionString%"       
                            },
                            new CMExecuteSQLNonQuery()
                            {
                                 ScriptFilePath = @"%PACKAGE_CONTENT%\SCRIPTS\2_Update_OuterConrol.sql",
                                 CommandType = System.Data.CommandType.Text,
                                 ConnectionString = "%sql_connection.ConnectionString%"       
                            },
                            new CMExecuteSQLNonQuery()
                            {
                                 ScriptFilePath = @"%PACKAGE_CONTENT%\SCRIPTS\3_Replace_ObjectCode.sql",
                                 CommandType = System.Data.CommandType.Text,
                                 ConnectionString = "%sql_connection.ConnectionString%"       
                            },
                            new CMExecuteSQLNonQuery()
                            {
                                 ScriptFilePath = @"%PACKAGE_CONTENT%\SCRIPTS\4_Update_FilterValue_RAP.sql",
                                 CommandType = System.Data.CommandType.Text,
                                 ConnectionString = "%sql_connection.ConnectionString%"       
                            },
                            new CMExecuteSQLNonQuery()
                            {
                                 ScriptFilePath = @"%PACKAGE_CONTENT%\SCRIPTS\5_UpdateModelFilters.sql",
                                 CommandType = System.Data.CommandType.Text,
                                 ConnectionString = "%sql_connection.ConnectionString%"       
                            },
                            new CMExecuteSQLNonQuery()
                            {
                                 ScriptFilePath = @"%PACKAGE_CONTENT%\SCRIPTS\6_BuildFormulas.sql",
                                 CommandType = CommandType.Text,
                                 ConnectionString = "%sql_connection.ConnectionString%"
                            }/*,
                            new CMExecuteSQLNonQuery()
                            {
                                 ScriptFilePath = @"%PACKAGE_CONTENT%\SCRIPTS\7_UpdateModelFilters.sql",
                                 CommandType = CommandType.Text,
                                 ConnectionString = "%sql_connection.ConnectionString%"
                            },
                            new CMExecuteSQLNonQuery()
                            {
                                 ScriptFilePath = @"%PACKAGE_CONTENT%\SCRIPTS\8_BuildFormulas.sql",
                                 CommandType = CommandType.Text,
                                 ConnectionString = "%sql_connection.ConnectionString%"
                            }*/
                       }
                    }
                }
            };

            return body;
        }

        /// <summary>
        /// Пакет апдейтера
        /// </summary>
        public static PackageBody CreatePackageUpdaterTestXLT()
        {
            PackageBody body = new PackageBody();
            body.PackageName = "Обновление 20120216_UPDATE_RAP";
            body.Command = new CommandSequence()
            {
                Commands = new List<Command>() 
                {
                    new UserInputForm()
                    {
                         InputControls= new List<UserInputControl>()
                         {
                             new ICSQLConnection()
                             {
                                  InitialValue= new SQLConnectionProperties()
                                  {
                                       DBName = "GRBS_437",
                                       InstanceName = ".",
                                       UserName = "sa",
                                       SqlAuthentication = true,
                                       Password = "osbb"
                                  },
                                  ResultName = "sql_connection"
                             }
                         },
                         FormCaption = "Подключение к БД",
                         AfterComplete = new CMParseResult()
                         {
                             ArgumentName = "%sql_connection%"
                         }
                    },
                    new CommandSequence()
                    {
                        Commands = new List<Command>()
                        {
                            new CreateDirectoryContent()
                            {
                                SourcePath = @"D:\20120216_UPDATE_RAP\FILES\",
                                DestPath = @"%PACKAGE_CONTENT%\FILES",
                                MoveFast = true
                            },
                            new CMSynchronize()
                            {
                                 ConnectionString = "%sql_connection.ConnectionString%",
                                 ConnectionStringEtalon = "",
                                 DTSDirectoryPath = @"%PACKAGE_CONTENT%\DTS",
                                 SyncType = OSBBSynchronizationType.TextFile,
                                 CheckFormulaDiapasons = false, 
                                 LoadHierarchy = false,
                                 UpdateSetStrings = false,
                                 PackagePath = @"%PACKAGE_CONTENT%\FILES\29", 
                                 LoadPick = false, 
                                 UpdateDisplayCode = false
                            }
                        }
                    }
                }
            };

            return body;
        }

        #endregion


    }
}
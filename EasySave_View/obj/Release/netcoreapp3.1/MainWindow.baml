   M S B A M L   `   `   `  ���� H  DEasySave_View, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:EasySave_ViewEasySave_View  ? ;Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5&clr-namespace:Core.Model;assembly=Core
Core.Model C&clr-namespace:EasySave_View.PropertiesEasySave_View.Properties      EasySave_View.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     -localclr-namespace:EasySave_View     8model&clr-namespace:Core.Model;assembly=Core     ;language&clr-namespace:EasySave_View.Properties       � Title$  EasySave��$	��450��$	��800�� � 	Resources !  Core.Model.StateFileContent   stc��%   &	          .���  � RowDefinitions �� $
��Auto ��� $
��Auto �.+�]� $��0��.+�o� $	��Top=�.@�m� -   $��
header_run��$	��Run��m� -   $��header_config��$��Configuration��.@�m� -   $��header_create��$��Create��m� -   $��header_delete��$��Delete��m� -   $��header_edit��$
��Edit��m� -   $	��job��$��Choose job application��m� -   $��crypto_header��$��Cryptosoft extension��m� -   $��priority_header��$��Priority file extensions��m� -	   $��sizeFile_header��$-��'Maximum file size for parallel transfer��m� -
   $��header_logs��$
��Logs��.@�m� -   $��header_logFile��$��Log File��m� -   $��header_stateFile��$��
State File��m� -   $��header_about��$��About us��m� -   $��header_language��$��Language��.@�m� -   $��header_english��$��English��m� -   $��header_french��$��French��m� -   $��header_quit��$
��Quit��� -   $��	stateView��$y�	Collapsed=�$��1�� �� $��* ��� $��* �.+��� $��0�� �� Orientation$ 
Horizontal=�$��Center=�$��10,10q�.+��� -   $��btnPlay��$��Pause��$��Center=�$	��Top=�$��0q�$��20��$��50��$	��1,0q��� -   $��btnPause��$
��Play��$
��Left=�$��0q�$��20��$��50��$	��2,0q��� -   $��btnStop��$
��Stop��$
��Left=�$��0q�$��20��$��50��$	��2,0q�� -   $��state��$��1��� -   $	��run��$��1��$y�Visible=� �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� -   $��Savework_to_run��$ ��Choose the savework to run��$��0��$��Center=�$��Center=�$��	0,10,0,55q�$	��250���� -   $��listSaveWorks��$��
51,80,0,55q�$
��Left=�$	��678��$��4���� -   $��btnChooseValidate��$��Validate��$��3��$��Center=�$��	0,186,0,0q�$	��Top=�$	��140��$��1�� J� RenderTransformOrigin$ 0.5,0.5&�$��34��� -   $��create��$��1��$y�	Collapsed=� � ColumnDefinitions �� $	��69* ��� $
��731* �.+��� -   $��
create_lbl��$��Create a save work��$
��Left=�$��276,4,0,321q�$��1���� -   $��name_lbl��$��Name of the savework��$
��Left=�$��
143,74,0,0q�$	��Top=�$��1���� -   $��
source_lbl��$��Source Folder��$
��Left=�$��143,128,0,0q�$	��Top=�$��1���� -   $��dest_lbl��$��Destination Folder��$
��Left=�$��143,182,0,0q�$	��Top=�$��1���� -    $��type_lbl��$
��Type��$
��Left=�$��143,236,0,0q�$	��Top=�$��1���� -!   $��txtName��$
��Left=�$��
331,74,0,0q�$��Enter the name�� �� TextWrapping$
 Wrap=�$	��Top=�$	��175��$��1���� -"   $��	txtSource��$
��Left=�$��331,128,0,0q�$��Choose the source folder��$
 Wrap=�$	��Top=�$	��175��$��1���� -#   $��txtDest��$
��Left=�$��331,182,0,0q�$#��Choose the destination folder��$
 Wrap=�$	��Top=�$	��175��$��1���� -$   $��	btnSource��$	��...��$
��Left=�$��519,128,0,0q�$	��Top=�$��23��$��1���� -%   $��btnDest��$	��...��$
��Left=�$��519,182,0,0q�$	��Top=�$��23��$��1���� -&   $��cmbType��$
��Left=�$��331,238,0,0q�$	��Top=�$	��175��$��1���� -'   $��btnValidate��$��Validate��$
��Left=�$��276,329,0,0q�$	��Top=�$	��109��$��1���� -(   $��
chkEncrypt��$��Encrypt��$��1��$
��Left=�$��300,300,0,0q�$	��Top=�� -)   $
��edit��$��1��$y�	Collapsed=�.+��� -*   $��	lblComing��$��Coming soon...��$��Center=�$��4q�� -+   $��delete��$��1��$y�	Collapsed=� �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� -,   $��lblchoosedelete��$!��Choose a savework to delete��$��1��$
��Left=�$��334,3,0,351q�$	��168��$��3���� --   $��listSaveWorksDelete��$��Visible=�$��1��$��22,41,0,-333q�$��2��$
��Left=�$	��759��$	��300���� -.   $��btnChooseValidateDelete��$��Validate��$��3��$
��Left=�$��350,338,0,0q�$	��Top=�$	��126��$��1��� -/   $��about��$
��auto��$��1��$y�	Collapsed=�.+�� $
��auto��$��50,30q�$��Center=�$��Center=�.+��� -0   $��txtBlckSoftName��$��Center=�$��EasySave soft��$��50(��� -1   $��lblAbout��$��Center=�$
��Wrap=�$
��auto��$����This project has been realized in the system programming project in third year's IT specialty class at CESI Bordeaux. Made By group 6 (Clément Courtembert Gaillard, Elouan Jeannot, Maxime Ferreira, Mathéo Berger).��$��Center=�$��4q�� -2   $��crypto��$��1��$y�	Collapsed=�.+��� -3   $��
crypto_lbl��$��Choose your extension��$��Center=�$��	0,4,0,360q��� -4   $��btnCryptoValidate��$��Validate��$
��Left=�$��241,317,0,0q�$	��Top=�$ 
0.478,2.76&�$	��118��$��29���� -5   $��
listCrypto��$��Visible=� �� SelectionMode$ Multiple=�$��32,46,668,46q�$	��100��$	��300���� -6   $��lblExtension��$��Center=�$8��2Your extension is not in the list? You can type it��$
��Left=�$��
162,63,0,0q�$	��Top=�$	��276���� -7   $��txtExtension��$
��Left=�$��241,108,0,0q�$�� ��$
 Wrap=�$	��Top=�$	��118��$��32���� -8   $��listExtension��$��Visible=�$ Multiple=�$��512,50,188,42q�$	��100��$	��300���� -9   $��btnExtensionDelete��$��Delete��$
��Left=�$��	651,0,0,0q�$��Center=�$ 
0.478,2.76&�$	��118��$��29��� -:   $��priorityFiles��$��1��$y�	Collapsed=�.+��� -;   $��priority_lbl��$)��#Choose your priority file extension��$��Center=�$��	0,4,0,360q��� -<   $��btnPriorityValidate��$��Validate��$
��Left=�$��241,317,0,0q�$	��Top=�$ 
0.478,2.76&�$	��118��$��29���� -=   $��listPriority��$��Visible=�$ Multiple=�$��32,46,668,46q�$	��100��$	��300���� ->   $��lblPriorityExtension��$��Center=�$!��Insert a specific extension��$
��Left=�$��
162,63,0,0q�$	��Top=�$	��276���� -?   $��txtPriorityExtension��$
��Left=�$��241,108,0,0q�$�� ��$
 Wrap=�$	��Top=�$	��118��$��32���� -@   $��listPriorityExtension��$��Visible=�$ Multiple=�$��512,50,188,42q�$	��100��$	��300���� -A   $ ��btnPriorityExtensionDelete��$��Delete��$
��Left=�$��	651,0,0,0q�$��Center=�$ 
0.478,2.76&�$	��118��$��29��� -B   $��fileSize��$��1��$y�	Collapsed=�.+��� -C   $��fileSize_lbl��$8��2Choose the maximal file size to tranfer (in bytes)��$��Center=�$��
0,10,0,354q��� -D   $��fileSize_txt��$
��Left=�$��238,106,0,0q�$�� ��$
 Wrap=�$	��Top=�$	��118��$��32���� -E   $��fileSize_button��$��Validate��$
��Left=�$��462,106,0,0q�$	��Top=�$ 
0.478,2.76&�$	��118��$��29���� -F   $��fileSize_lbl2��$��Size chosen :��$
��Left=�$��238,203,0,250q��� -G   $��fileSize_lblSize��$��0 bytes��$
��Left=�$��321,203,0,250q�
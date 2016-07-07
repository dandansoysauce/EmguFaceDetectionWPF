# EmguFaceDetectionWPF
A Face Detection App in WPF using Emgu CV. This app aims to be a base app for all your face detection needs. 

**Features:**
* MVVM Pattern for scalibility.
* SQLite Database, no further configs. You can still migrate to other database engines. The SQL structure's at `DbConstants`
* Ability to take **Attendance Logs**
* Ability to add user.
* Ability to view logs, and filter.

**ToDo:**
* Add ability to edit users
* Add ability to view user's image on logs view

**Images:**
![Monitoring View][sc1]
![Logs View][sc2]
![Add User][sc3]

**References:**
* [Emgu CV][Ref 1] - face detection engine
* [Material Design In XAML Toolkit][Ref 2] - main app theme
* [System.Data.SQLite][Ref 3] - database engine

[sc1]: https://raw.githubusercontent.com/zxcdani/EmguFaceDetectionWPF/master/Images/screen1.png
[sc2]: https://raw.githubusercontent.com/zxcdani/EmguFaceDetectionWPF/master/Images/screen2.png
[sc3]: https://raw.githubusercontent.com/zxcdani/EmguFaceDetectionWPF/master/Images/screen3.png
[Ref 1]: http://www.emgu.com/wiki/index.php/Main_Page
[Ref 2]: https://github.com/ButchersBoy/MaterialDesignInXamlToolkit
[Ref 3]: https://system.data.sqlite.org/index.html/doc/trunk/www/index.wiki

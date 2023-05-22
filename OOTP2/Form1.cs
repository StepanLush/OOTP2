using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace OOTP2
{
    public partial class Form1 : Form
    {
        private System.Windows.Forms.DataGridView dataGridView  = new DataGridView();
        List<System.Windows.Forms.RadioButton> radioButtons = new List<System.Windows.Forms.RadioButton>();
        private Button addButton;
        private Button editButton;
        private Button deleteButton;

        public Form1()
        {
            InitializeComponent();
            Book book1 = new Book();
            book1.Name = "Book 1";
            book1.Genre = "Genre 1";
            book1.Place = "Place 1";

            Book book2 = new Book();
            book2.Name = "Book 2";
            book2.Genre = "Genre 2";
            book2.Place = "Place 2";

            Person person1 = new Person();
            person1.Name = "Person 1";
            person1.Email = "Email 1";
            person1.Age = 25;

            Person person2 = new Person();
            person2.Name = "Person 2";
            person2.Email = "Email 2";
            person2.Age = 30;

            User user1 = new User();
            user1.Name = "User 1";
            user1.Email = "Email 1";
            user1.Age = 25;
            user1.IsDept = false;
            user1.BookList = new List<Book> { book1 };

            User user2 = new User();
            user2.Name = "User 2";
            user2.Email = "Email 2";
            user2.Age = 30;
            user2.IsDept = true;
            user2.BookList = new List<Book> { book2 };

            Author author1 = new Author();
            author1.Name = "Author 1";
            author1.Email = "Email 1";
            author1.Age = 40;
            author1.IsAlive = true;

            Author author2 = new Author();
            author2.Name = "Author 2";
            author2.Email = "Email 2";
            author2.Age = 50;
            author2.IsAlive = false;

            Worker worker1 = new Worker();
            worker1.Name = "Worker 1";
            worker1.Email = "Email 1";
            worker1.Age = 35;
            worker1.Salary = 5000;
            worker1.Place = "Place 1";

            Worker worker2 = new Worker();
            worker2.Name = "Worker 2";
            worker2.Email = "Email 2";
            worker2.Age = 45;
            worker2.Salary = 6000;
            worker2.Place = "Place 2";

            Security security1 = new Security();
            security1.Name = "Security 1";
            security1.Email = "Email 1";
            security1.Age = 30;
            security1.Salary = 4000;
            security1.Place = "Place 1";
            security1.Shift = 1;

            Security security2 = new Security();
            security2.Name = "Security 2";
            security2.Email = "Email 2";
            security2.Age = 35;
            security2.Salary = 4500;
            security2.Place = "Place 2";
            security2.Shift = 2;

            // Adding instances to their respective managers
            Managers.BookManager.AddBook(book1);
            Managers.BookManager.AddBook(book2);

            Managers.PersonManager.AddPerson(person1);
            Managers.PersonManager.AddPerson(person2);

            Managers.UserManager.AddUser(user1);
            Managers.UserManager.AddUser(user2);

            Managers.AuthorManager.AddAuthor(author1);
            Managers.AuthorManager.AddAuthor(author2);

            Managers.WorkerManager.AddWorker(worker1);
            Managers.WorkerManager.AddWorker(worker2);

            Managers.SecurityManager.AddSecurity(security1);
            Managers.SecurityManager.AddSecurity(security2);

            // Accessing the instances through the managers
            List<Book> books = Managers.BookManager.GetBooks();
            List<Person> persons = Managers.PersonManager.GetPersons();
            List<User> users = Managers.UserManager.GetUsers();
            List<Author> authors = Managers.AuthorManager.GetAuthors();
            List<Worker> workers = Managers.WorkerManager.GetWorkers();
            List<Security> securities = Managers.SecurityManager.GetSecurities();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeButtons();
        }

        private void InitializeButtons()
        {
            string classesFolder = "Classes";
            string[] classFiles = Directory.GetFiles(classesFolder, "*.cs");

            // Создание радиокнопок
            int top = 10;
            foreach (string classFile in classFiles)
            {
                string className = Path.GetFileNameWithoutExtension(classFile);
                RadioButton radioButton = new System.Windows.Forms.RadioButton();
                radioButton.Text = className;
                radioButton.Location = new Point(10, top);
                radioButton.Tag = className; // Установка значения Tag равным имени класса
                radioButtons.Add(radioButton);
                Controls.Add(radioButton);
                top += 25;
            }
            foreach (RadioButton radioButton in radioButtons)
            {
                radioButton.CheckedChanged += RadioButton_CheckedChanged;
            }


            // Создание кнопок
            addButton = new Button();
            addButton.Text = "Добавить";
            addButton.Location = new Point(10, top);
            addButton.Click += AddButton_Click;
            Controls.Add(addButton);

            editButton = new Button();
            editButton.Text = "Редактировать";
            editButton.Location = new Point(100, top);
            editButton.Click += EditButton_Click;
            Controls.Add(editButton);

            deleteButton = new Button();
            deleteButton.Text = "Удалить";
            deleteButton.Location = new Point(190, top);
            deleteButton.Click += DeleteButton_Click;
            Controls.Add(deleteButton);
            top += 25;

            dataGridView.Location = new Point(10, top);
            dataGridView.Size = new Size(600, 300);
            Controls.Add(dataGridView);


        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            Type objectType = GetSelectedObjectType();
            if (objectType != null)
            {
                var managerProperty = typeof(Managers).GetProperty($"{objectType.Name}Manager");
                if (managerProperty != null)
                {
                    var manager = managerProperty.GetValue(null);
                    var deleteMethod = manager.GetType().GetMethod("Delete" + objectType.Name);
                    if (deleteMethod != null)
                    {
                        object selectedObject = GetSelectedObjectFromManager(objectType);
                        if (selectedObject != null)
                        {
                            DialogResult result = MessageBox.Show("Вы уверены, что хотите удалить выбранный объект?", "Удаление объекта", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (result == DialogResult.Yes)
                            {
                                DeleteObject(selectedObject); 
                                deleteMethod.Invoke(manager, new[] { selectedObject });
                                RefreshDataGridView(); 
                            }
                        }
                        else
                        {
                            MessageBox.Show("Пожалуйста, выберите объект для удаления.");
                        }
                    }
                }
            }
        }

        private void DeleteObject(object obj)
        {
            if (obj != null)
            {
                Type objectType = obj.GetType();
                foreach (var field in objectType.GetFields())
                {
                    // Проверяем, является ли поле классом (не строкой)
                    if (field.FieldType.IsClass && field.FieldType != typeof(string))
                    {
                        var fieldValue = field.GetValue(obj);
                        if (fieldValue != null)
                        {
                            // Рекурсивно вызываем DeleteObject для каждого поля класса
                            DeleteObject(fieldValue);
                        }
                    }

                    // Обнуляем значение поля
                    field.SetValue(obj, null);
                }
            }
        }






        private void RefreshDataGridView()//не смотреть, ужасный код
        {
            Type objectType = GetSelectedObjectType();
            if (objectType != null)
            {
                var managerProperty = typeof(Managers).GetProperty($"{objectType.Name}Manager");
                if (managerProperty != null)
                {
                    var manager = managerProperty.GetValue(null);
                    var getObjectsMethod = manager.GetType().GetMethod("Get" + objectType.Name + "s");
                    if (getObjectsMethod != null)
                    {
                        var objects = getObjectsMethod.Invoke(manager, null) as IList;
                        if (objects != null)
                        {
                            dataGridView.Rows.Clear();

                            if (objects.Count > 0)
                            {
                                foreach (var obj in objects)
                                {
                                    var rowData = new List<object>();
                                    foreach (var field in objectType.GetFields())
                                    {
                                        if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                                        {
                                            // Если поле является списком, добавляем имена каждого элемента списка
                                            var list = field.GetValue(obj) as IList;
                                            if (list != null)
                                            {
                                                var names = list.Cast<dynamic>().Select(item => item.Name).ToList();
                                                rowData.Add(string.Join(", ", names));
                                            }
                                            else
                                            {
                                                rowData.Add(field.GetValue(obj));
                                            }
                                        }

                                        // Если поле является классом (не строкой), добавляем имя объекта этого класса
                                        else if (field.FieldType.IsClass && field.FieldType != typeof(string))
                                        {
                                            var fieldValue = field.GetValue(obj);
                                            if (fieldValue != null)
                                            {
                                                var classNameField = field.FieldType.GetField("Name");
                                                if (classNameField != null)
                                                {
                                                    var className = classNameField.GetValue(fieldValue);
                                                    rowData.Add(className);
                                                }
                                                else
                                                {
                                                    rowData.Add(fieldValue);
                                                }
                                            }
                                            else
                                            {
                                                rowData.Add(null); // Добавляем null в поле данных, если поле класса пустое
                                            }
                                        }
                                        else
                                        {
                                            rowData.Add(field.GetValue(obj));
                                        }
                                    }
                                    dataGridView.Rows.Add(rowData.ToArray());
                                }
                                
                            }
                        }
                    }
                }
            }
        }
        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is System.Windows.Forms.RadioButton radioButton && radioButton.Checked)
            {
                string typeFullName = "OOTP2." + radioButton.Tag.ToString();
                Type objectType = Type.GetType(typeFullName);
                if (objectType != null)
                {
                    var managerProperty = typeof(Managers).GetProperty($"{objectType.Name}Manager");
                    if (managerProperty != null)
                    {
                        var manager = managerProperty.GetValue(null);
                        var getObjectsMethod = manager.GetType().GetMethod("Get" + objectType.Name + "s");
                        if (getObjectsMethod != null)
                        {
                            var objects = getObjectsMethod.Invoke(manager, null) as IList;
                            if (objects != null)
                            {
                                dataGridView.Columns.Clear();

                                // Добавление столбцов на основе полей объекта
                                foreach (var field in objectType.GetFields())
                                {
                                    dataGridView.Columns.Add(field.Name, field.Name);
                                }

                                if (objects.Count > 0)
                                {
                                    // Заполнение данных в таблице
                                    dataGridView.Rows.Clear();
                                    foreach (var obj in objects)
                                    {
                                        var rowData = new List<object>();
                                        foreach (var field in objectType.GetFields())
                                        {
                                            if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                                            {
                                                // Если поле является списком, добавляем имена каждого элемента списка
                                                var list = field.GetValue(obj) as IList;
                                                if (list != null)
                                                {
                                                    var names = list.Cast<dynamic>().Select(item => item.Name).ToList();
                                                    rowData.Add(string.Join(", ", names));
                                                }
                                                else
                                                {
                                                    rowData.Add(field.GetValue(obj));
                                                }
                                            }

                                            // Если поле является классом (не строкой), добавляем имя объекта этого класса
                                            else if (field.FieldType.IsClass && field.FieldType != typeof(string))
                                            { 
                                                var fieldValue = field.GetValue(obj);
                                                if (fieldValue != null)
                                                {
                                                    var classNameField = field.FieldType.GetField("Name");
                                                    if (classNameField != null)
                                                    {
                                                        var className = classNameField.GetValue(fieldValue);
                                                        rowData.Add(className);
                                                    }
                                                    else
                                                    {
                                                        rowData.Add(fieldValue);
                                                    }
                                                }
                                                else
                                                {
                                                    rowData.Add(null); // Добавляем null в поле данных, если поле класса пустое
                                                }
                                            }

                                            
                                            else
                                            {
                                                rowData.Add(field.GetValue(obj));
                                            }
                                        }
                                        dataGridView.Rows.Add(rowData.ToArray());
                                    }

                                }
                                else
                                {
                                    dataGridView.Rows.Clear();
                                }

                            }
                        }
                    }
                }
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            Type objectType = GetSelectedObjectType();
            if (objectType != null)
            {
                var managerProperty = typeof(Managers).GetProperty($"{objectType.Name}Manager");
                if (managerProperty != null)
                {
                    var manager = managerProperty.GetValue(null);
                    var addMethod = manager.GetType().GetMethod("Add" + objectType.Name);
                    if (addMethod != null)
                    {
                        AddEditForm addEditForm = new AddEditForm(objectType, FormMode.Add);
                        if (addEditForm.ShowDialog() == DialogResult.OK)
                        {
                            object createdObject = addEditForm.CreatedObject;
                            addMethod.Invoke(manager, new[] { createdObject });
                            RefreshDataGridView();
                        }
                    }
                }
            }
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            Type objectType = GetSelectedObjectType();
            if (objectType != null)
            {
                var managerProperty = typeof(Managers).GetProperty($"{objectType.Name}Manager");
                if (managerProperty != null)
                {
                    var manager = managerProperty.GetValue(null);
                    var updateMethod = manager.GetType().GetMethod("Update" + objectType.Name);
                    if (updateMethod != null)
                    {
                        object selectedObject = GetSelectedObjectFromManager(objectType);
                        if (selectedObject != null)
                        {
                            AddEditForm addEditForm = new AddEditForm(objectType, FormMode.Edit, selectedObject);
                            if (addEditForm.ShowDialog() == DialogResult.OK)
                            {
                                object updatedObject = addEditForm.CreatedObject;
                                updateMethod.Invoke(manager, new[] { updatedObject });
                                RefreshDataGridView();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Please select an object to edit.");
                        }
                    }
                }
            }
        }

        private object GetSelectedObjectFromManager(Type objectType)
        {
            var managerProperty = typeof(Managers).GetProperty($"{objectType.Name}Manager");
            if (managerProperty != null)
            {
                var manager = managerProperty.GetValue(null);
                var getObjectsMethod = manager.GetType().GetMethod("Get" + objectType.Name + "s");
                if (getObjectsMethod != null)
                {
                    var objects = getObjectsMethod.Invoke(manager, null) as IList;
                    if (objects != null && objects.Count > 0)
                    {
                        int selectedIndex = dataGridView.SelectedCells[0].RowIndex;
                        if (selectedIndex >= 0 && selectedIndex < objects.Count)
                        {
                            return objects[selectedIndex];
                        }
                    }
                }
            }

            return null;
        }


        private Type GetSelectedObjectType()
        {
            foreach (System.Windows.Forms.RadioButton radioButton in radioButtons)
            {
                if (radioButton.Checked)
                {
                    string className = radioButton.Text;
                    Type objectType = Type.GetType($"OOTP2.{className}");
                    return objectType;
                }
            }

            return null; 
        }

    }
}

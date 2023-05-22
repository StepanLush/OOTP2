using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OOTP2
{
    public enum FormMode
    {
        Add,
        Edit
    }

    public partial class AddEditForm : Form
    {
        public FormMode Mode { get; set; }
        private Type objectType;
        private object editedObject;
        private Dictionary<string, Control> fieldValueControls;
        
        private List<object> selectedObjects;
        private object selectedObject;
        public object CreatedObject { get; private set; }

        public AddEditForm(Type objectType, FormMode mode, object editedObject = null)
        {
            this.objectType = objectType;
            this.Mode = mode;
            this.editedObject = editedObject;
            fieldValueControls = new Dictionary<string, Control>();
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            // Label для отображения заголовка формы
            Label titleLabel = new Label();
            titleLabel.Text = Mode == FormMode.Add ? $"Добавление {objectType.Name}" : $"Редактирование {objectType.Name}";
            titleLabel.Location = new Point(10, 10);
            titleLabel.AutoSize = true;
            Controls.Add(titleLabel);

            // Получение списка полей класса
            var fields = objectType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            int top = 40;

            // Создание и настройка элементов управления для каждого поля
            foreach (var field in fields)
            {
                // Label для отображения имени поля
                Label fieldLabel = new Label();
                fieldLabel.Text = field.Name + ":";
                fieldLabel.Location = new Point(10, top);

                // Создание элемента управления в зависимости от типа поля
                Control fieldValueControl = null;
                if (field.FieldType == typeof(string))
                {
                    // TextBox для ввода строки
                    TextBox fieldValueTextBox = new TextBox();
                    fieldValueControl = fieldValueTextBox;

                }
                else if (field.FieldType == typeof(int))
                {
                    // NumericUpDown для ввода целого числа
                    NumericUpDown fieldValueNumericUpDown = new NumericUpDown();
                    fieldValueControl = fieldValueNumericUpDown;

                }
                else if (field.FieldType == typeof(bool))
                {
                    // CheckBox для ввода булевого значения
                    CheckBox fieldValueCheckBox = new CheckBox();
                    fieldValueControl = fieldValueCheckBox;
                }
                else if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    Button addExistingButton = new Button();
                    addExistingButton.Text = "Редактировать";
                    addExistingButton.Location = new Point(120, top);
                    addExistingButton.Tag = field.Name;
                    addExistingButton.Click += AddExistingButton_Click;

                    Button createNewButton = new Button();
                    createNewButton.Text = "Создать";
                    createNewButton.Location = new Point(240, top);
                    createNewButton.Tag = field.Name;
                    createNewButton.Click += CreateNewButton_Click;

                    Controls.Add(fieldLabel);
                    Controls.Add(addExistingButton);
                    Controls.Add(createNewButton);
                    
                    top += 30;
                    continue; // Пропускаем создание элемента управления для поля-списка
                }
                else if (field.FieldType.IsClass)
                {
                    Button selectButton = new Button();
                    selectButton.Text = "Выбрать";
                    selectButton.Location = new Point(120, top);
                    selectButton.Tag = field.Name;
                    selectButton.Click += SelectButton_Click;

                    Button editButton = new Button();
                    editButton.Text = "Создать";
                    editButton.Location = new Point(240, top);
                    editButton.Tag = field.Name;
                    editButton.Click += CreateNewButton2_Click;

                    Controls.Add(fieldLabel);
                    Controls.Add(editButton);
                    Controls.Add(selectButton);

                    top += 30;

                }

                if (fieldValueControl != null)
                {
                    fieldValueControl.Location = new Point(120, top);
                    fieldValueControl.Size = new Size(200, 20);
                    fieldValueControl.Tag = field.Name;

                    Controls.Add(fieldLabel);
                    Controls.Add(fieldValueControl);

                    top += 30;

                    if (editedObject != null && Mode == FormMode.Edit)
                    {
                        // Заполняем элементы управления значениями из редактируемого объекта
                        var value = field.GetValue(editedObject);
                        if (fieldValueControl is TextBox textBox)
                            textBox.Text = value?.ToString();
                        else if (fieldValueControl is NumericUpDown numericUpDown)
                            numericUpDown.Value = Convert.ToDecimal(value);
                        else if (fieldValueControl is CheckBox checkBox)
                            checkBox.Checked = Convert.ToBoolean(value);
                    }

                    fieldValueControls[field.Name] = fieldValueControl;
                }
            }

            // Создание кнопок сохранения и отмены
            Button saveButton = new Button();
            saveButton.Text = "Сохранить";
            saveButton.Location = new Point(20, top);
            saveButton.Click += SaveButton_Click;

            Button cancelButton = new Button();
            cancelButton.Text = "Отмена";
            cancelButton.Location = new Point(120, top);
            cancelButton.Click += CancelButton_Click;

            Controls.Add(saveButton);
            Controls.Add(cancelButton);
        }


        private void SaveButton_Click(object sender, EventArgs e)
        {

            // Создание нового объекта или обновление существующего в зависимости от режима
            object newObject;
            if (Mode == FormMode.Add)
            {
                newObject = Activator.CreateInstance(objectType);
            }
            else
            {
                newObject = editedObject;
            }

            // Заполнение полей объекта значениями из элементов управления
            foreach (var kvp in fieldValueControls)
            {
                var fieldName = kvp.Key;
                var fieldValueControl = kvp.Value;

                var field = objectType.GetField(fieldName);
                var fieldType = field.FieldType;

                object fieldValue = null;
                if (fieldValueControl is TextBox textBox)
                {
                    if (textBox.Text == "")
                    {
                        MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                        fieldValue = textBox.Text;
                    }
                }
                else if (fieldValueControl is NumericUpDown numericUpDown)
                {
                    fieldValue = Convert.ChangeType(numericUpDown.Value, fieldType);
                }
                else if (fieldValueControl is CheckBox checkBox)
                {
                    fieldValue = checkBox.Checked;
                }

                field.SetValue(newObject, fieldValue);
            }

            var fields = objectType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var field in fields)
            {
                object fieldValue = null;
                if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    if (selectedObjects != null)
                    {
                        fieldValue = selectedObjects.Cast<Book>().ToList();
                        field.SetValue(newObject, fieldValue);
                    }
                }
                else if (field.FieldType.IsClass && !field.FieldType.IsPrimitive && !field.FieldType.Namespace.StartsWith("System"))
                {
                    fieldValue = selectedObject;
                    field.SetValue(newObject, fieldValue);
                }

            }
            CreatedObject = newObject;
            DialogResult = DialogResult.OK;

            Close();
        }

        private void SelectButton_Click(object sender, EventArgs e)
        {
            // Получение имени поля-класса из тега кнопки
            string fieldName = (sender as Button)?.Tag?.ToString();

            if (!string.IsNullOrEmpty(fieldName))
            {
                // Получение типа класса из поля
                var field = objectType.GetField(fieldName);
                var classType = field.FieldType;

                // Получение списка уже существующих объектов класса
                List<object> existingObjects = GetObjectList(classType);

                // Создание экземпляра формы для отображения списка объектов
                var listForm = new ListForm(existingObjects);

                // Отображение формы со списком объектов
                if (listForm.ShowDialog() == DialogResult.OK)
                {
                    // Получение выбранного объекта из списка
                    selectedObject = listForm.selectedObject;

    
                }
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            CreatedObject = null;
            Close();
        }

        private void AddExistingButton_Click(object sender, EventArgs e)
        {
            // Получение имени поля-списка из тега кнопки
            string fieldName = (sender as Button)?.Tag?.ToString();

            if (!string.IsNullOrEmpty(fieldName))
            {
                // Получение типа списка из поля класса
                var field = objectType.GetField(fieldName);
                var listType = field.FieldType.GetGenericArguments()[0];

                // Получение значения поля-списка из редактируемого объекта
                var fieldValue = editedObject != null ? ((IEnumerable)field.GetValue(editedObject)).Cast<object>().ToList() : null;


                // Получение списка объектов
                List<object> objectList = GetObjectList(listType);

                // Создание экземпляра формы ListForm
                var listForm = new ListForm(objectList, fieldValue);

                // Отображение формы ListForm
                if (listForm.ShowDialog() == DialogResult.OK)
                {
                    selectedObjects = listForm.selectedObjects;
                }
            }
        }

        private void CreateNewButton_Click(object sender, EventArgs e)
        {
            // Получение имени поля-списка из тега кнопки
            string fieldName = (sender as Button)?.Tag?.ToString();

            if (!string.IsNullOrEmpty(fieldName))
            {
                // Получение типа списка из поля класса
                var field = objectType.GetField(fieldName);
                var listType = field.FieldType.GetGenericArguments()[0];

                // Получение менеджера списка
                var managerProperty = typeof(Managers).GetProperty($"{listType.Name}Manager");
                if (managerProperty != null)
                {
                    var manager = managerProperty.GetValue(null);
                    var addMethod = manager.GetType().GetMethod("Add" + listType.Name);
                    if (addMethod != null)
                    {
                        // Создание экземпляра формы AddEditForm для создания нового объекта
                        var addEditForm = new AddEditForm(listType, FormMode.Add);

                        // Отображение формы AddEditForm
                        if (addEditForm.ShowDialog() == DialogResult.OK)
                        {
                            // Получение созданного объекта из формы AddEditForm
                            object createdObject = addEditForm.CreatedObject;

                            if (createdObject != null)
                            {
                                // Добавление созданного объекта в поле-список
                                var fieldValue = editedObject != null ? (List<object>)field.GetValue(editedObject) : null;
                                fieldValue?.Add(createdObject);

                                // Добавление созданного объекта в глобальный список класса
                                addMethod.Invoke(manager, new[] { createdObject });
                            }
                        }
                    }
                }
            }
        }

        private void CreateNewButton2_Click(object sender, EventArgs e)
        {
            // Получение имени поля-класса из тега кнопки
            string fieldName = (sender as Button)?.Tag?.ToString();

            if (!string.IsNullOrEmpty(fieldName))
            {
                // Получение типа класса из поля
                var field = objectType.GetField(fieldName);
                var classType = field.FieldType;

                // Получение менеджера класса
                var managerProperty = typeof(Managers).GetProperty($"{classType.Name}Manager");
                if (managerProperty != null)
                {
                    var manager = managerProperty.GetValue(null);
                    var addMethod = manager.GetType().GetMethod("Add" + classType.Name);
                    if (addMethod != null)
                    {
                        // Создание экземпляра формы AddEditForm для создания нового объекта
                        var addEditForm = new AddEditForm(classType, FormMode.Add);

                        // Отображение формы AddEditForm
                        if (addEditForm.ShowDialog() == DialogResult.OK)
                        {
                            // Получение созданного объекта из формы AddEditForm
                            object createdObject = addEditForm.CreatedObject;

                            if (createdObject != null)
                            {
                                selectedObject = createdObject;

                                // Добавление созданного объекта в глобальный список класса
                                addMethod.Invoke(manager, new[] { createdObject });
                            }
                        }
                    }
                }
            }
        }



        private List<object> GetObjectList(Type objectType)
        {
            List<object> objectList = new List<object>();

            // Получение имени менеджера по типу объекта
            string managerPropertyName = $"{objectType.Name}Manager";
            var managerProperty = typeof(Managers).GetProperty(managerPropertyName);

            if (managerProperty != null)
            {
                // Получение менеджера объекта
                var manager = managerProperty.GetValue(null);

                // Получение метода "GetObjects" по типу объекта
                var getObjectsMethod = manager.GetType().GetMethod($"Get{objectType.Name}s");

                if (getObjectsMethod != null)
                {
                    // Вызов метода "GetObjects" для получения списка объектов
                    var objects = getObjectsMethod.Invoke(manager, null) as IEnumerable<object>;
                    if (objects != null)
                    {
                        objectList.AddRange(objects);
                    }
                }
            }

            return objectList;
        }

        

        private void AddEditForm_Load(object sender, EventArgs e)
        {

        }
    }

}

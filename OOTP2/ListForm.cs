using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Linq;

namespace OOTP2
{
    public partial class ListForm : Form
    {
        private List<object> allObjects;
        public List<object> selectedObjects;
        private DataGridView dataGridView;
        private Button confirmButton;

        public object selectedObject;
        public object editedObject;
        public ListForm(List<object> allObjects, List<object> selectedObjects)
        {
            this.allObjects = allObjects;
            this.selectedObjects = selectedObjects;

            InitializeComponents();
            PopulateDataGridView();
        }
        public ListForm(List<object> objects)
        {
            allObjects = objects;
            selectedObject = null;

            InitializeComponent2();
            PopulateDataGridView();
        }

        private void InitializeComponents()
        {
            dataGridView = new DataGridView();
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.AutoGenerateColumns = true;

            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.MultiSelect = true;
            Controls.Add(dataGridView);

            confirmButton = new Button();
            confirmButton.Text = "Подтвердить";
            confirmButton.Dock = DockStyle.Bottom;
            confirmButton.Click += ConfirmButton_Click;
            Controls.Add(confirmButton);
        }
        private void InitializeComponent2()
        {
            dataGridView = new DataGridView();
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.AutoGenerateColumns = true;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.MultiSelect = false;

            Controls.Add(dataGridView);

            confirmButton = new Button();
            confirmButton.Text = "Подтвердить";
            confirmButton.Dock = DockStyle.Bottom;
            confirmButton.Click += ConfirmButton_Click;
            Controls.Add(confirmButton);
        }

        private void PopulateDataGridView()
        {
            Type objectType = null;

            if (allObjects.Count > 0)
            {
                objectType = allObjects[0].GetType();
            }

            if (objectType == null)
            {
                // Если список пуст или все элементы null, невозможно определить тип объекта
                return;
            }

            // Создание и настройка столбцов таблицы на основе полей класса
            var columns = objectType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var column in columns)
            {
                dataGridView.Columns.Add(column.Name, column.Name);
            }

            // Заполнение строк таблицы значениями полей объектов
            foreach (var obj in allObjects)
            {
                var rowData = new List<object>();
                foreach (var field in objectType.GetFields())
                {
                    if (field.FieldType.IsClass && field.FieldType != typeof(string))
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

            // Пометка выбранных объектов в таблице
            if (selectedObjects != null)
            {
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    var obj = row.DataBoundItem;
                    bool isSelected = selectedObjects.Contains(obj);
                    row.Cells[0].Value = isSelected;

                    if (row.Cells[0] is DataGridViewCheckBoxCell checkBoxCell)
                    {
                        checkBoxCell.Value = isSelected;
                    }
                }
            }

        }




        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            if (selectedObjects != null)
            {
                selectedObjects.Clear();
            }
            else
            {
                selectedObjects = new List<object>();
            }

            if (dataGridView.SelectedRows.Count > 0)
            {
                // Получение выбранного объекта из выделенной строки таблицы
                var selectedRow = dataGridView.SelectedRows[0];
                selectedObject = allObjects[selectedRow.Index];

                foreach (DataGridViewRow row in dataGridView.SelectedRows)
                {
                    object obj = allObjects[row.Index];
                    selectedObjects.Add(obj);
                }

            }

            DialogResult = DialogResult.OK;
        }

        private void ListForm_Load(object sender, EventArgs e)
        {

        }
    }
}

using Messenger.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Messenger.Client
{
    public partial class EditDepartmentForm : Form
    {
        public Department Department { get; private set; }

        public EditDepartmentForm(Department department = null)
        {
            InitializeComponent();

            if (department != null)
            {
                Department = department;
                txtName.Text = department.Name;
                txtDescription.Text = department.Description;
                Text = "Редактирование отдела";
            }
            else
            {
                Department = new Department();
                Text = "Добавление отдела";
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название отдела.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Department.Name = txtName.Text.Trim();
            Department.Description = txtDescription.Text.Trim();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}

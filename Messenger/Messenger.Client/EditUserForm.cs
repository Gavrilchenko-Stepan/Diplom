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
    public partial class EditUserForm : Form
    {
        private User editingUser;
        private List<Department> departments;
        private NetworkClient networkClient;
        private bool isNewUser;
        private int? oldDepartmentId;

        public EditUserForm(User user, List<Department> depts, NetworkClient client)
        {
            InitializeComponent();
            UIHelper.SetRoundedRegion(this, 20);
            editingUser = user;
            departments = depts;
            networkClient = client;
            isNewUser = (user == null);

            LoadDepartments();

            if (!isNewUser)
            {
                oldDepartmentId = editingUser.DepartmentId;
                lblTitle.Text = "Редактирование пользователя";
                txtUsername.Text = editingUser.Username;
                txtFullName.Text = editingUser.FullName;
                txtPosition.Text = editingUser.Position;
                chkIsAdmin.Checked = editingUser.IsAdmin;
                if (editingUser.DepartmentId.HasValue)
                {
                    var selected = cmbDepartment.Items.Cast<dynamic>().FirstOrDefault(x => x.Id == editingUser.DepartmentId);
                    if (selected != null)
                        cmbDepartment.SelectedItem = selected;
                }
                else
                {
                    cmbDepartment.SelectedIndex = 0;
                }
            }
            else
            {
                lblTitle.Text = "Добавление пользователя";
            }
        }

        private void LoadDepartments()
        {
            var deptList = new List<dynamic>();  // можно использовать анонимный тип или List<object>
            deptList.Add(new { Id = (int?)null, Name = "Без отдела" });
            foreach (var d in departments)
            {
                deptList.Add(new { Id = (int?)d.Id, Name = d.Name });
            }

            cmbDepartment.DataSource = deptList;
            cmbDepartment.DisplayMember = "Name";
            cmbDepartment.ValueMember = "Id";
            cmbDepartment.SelectedIndex = -1;

            // Если редактируем существующего пользователя, нужно установить выбранное значение
            if (!isNewUser && editingUser.DepartmentId.HasValue)
            {
                // Ищем среди элементов тот, у которого Id равен editingUser.DepartmentId
                var selected = deptList.FirstOrDefault(x => x.Id == editingUser.DepartmentId);
                if (selected != null)
                    cmbDepartment.SelectedItem = selected;
            }
            else if (!isNewUser && !editingUser.DepartmentId.HasValue)
            {
                // Если у пользователя отдел не задан (null), выбираем "Без отдела"
                cmbDepartment.SelectedItem = deptList[0];
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Введите логин.");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Введите ФИ.");
                return;
            }
            if (cmbDepartment.SelectedItem == null)
            {
                MessageBox.Show("Выберите отдел.");
                return;
            }
            if (isNewUser && string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Введите пароль для нового пользователя.");
                return;
            }

            var user = new User
            {
                Id = editingUser?.Id ?? 0,
                Username = txtUsername.Text.Trim(),
                FullName = txtFullName.Text.Trim(),
                DepartmentId = cmbDepartment.SelectedValue as int?,
                Position = txtPosition.Text.Trim(),
                IsAdmin = chkIsAdmin.Checked
            };

            var packet = new NetworkPacket();
            if (isNewUser)
            {
                string hashedPassword = SecurityHelper.HashPassword(txtPassword.Text);
                packet.Command = Shared.CommandType.AddUser;
                packet.Data = new { user, password = hashedPassword };
            }
            else
            {
                string newPassword = string.IsNullOrWhiteSpace(txtPassword.Text)
                    ? null
                    : SecurityHelper.HashPassword(txtPassword.Text);
                var data = new Dictionary<string, object>
                {
                    { "user", user },
                    { "password", newPassword },
                    { "oldDepartmentId", oldDepartmentId }
                };
                packet.Command = Shared.CommandType.UpdateUser;
                packet.Data = data;
            }
            networkClient.SendPacket(packet);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}

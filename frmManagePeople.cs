using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Business;

namespace DVLD_2_my
{
    public partial class frmManagePeople : Form
    {
        
        private readonly string[] _FilterOptions = { "None", "Person ID", "National No.", "First Name", "Second Name",
                                                        "Third Name", "Last Name", "Nationality", "Gender", "Phone", "Email" };

        private DataTable _AllPeopleData = clsPerson.GetAllPeople();

        public frmManagePeople()
        {
            InitializeComponent();
        }

        private void btnAddPerson_Click(object sender, EventArgs e)
        {
            Form frm = new frmAddEditPerson();
            frm.ShowDialog();
        }

        private void _RefreshPeopleData()
        {
            
            if (_AllPeopleData == null)
                _AllPeopleData = clsPerson.GetAllPeople();

            cbFilterBy.Items.Clear();
            cbFilterBy.Items.AddRange(_FilterOptions);
            cbFilterBy.SelectedIndex = 0;

            dgvManagePeople.DataSource = _AllPeopleData;
            lblRecord.Text = "#Recorde: " + _AllPeopleData.Rows.Count.ToString();
        }

        private void frmManagePeople_Load(object sender, EventArgs e)
        {
            _RefreshPeopleData();
        }

        private void tbFilterBy_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (e.KeyChar == (char)Keys.Back || e.KeyChar == (char)Keys.Delete)
                return;
        }

        private void tbFilterBy_TextChanged(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(tbFilterBy.Text) && cbFilterBy.SelectedIndex != 0)
            {


                switch (cbFilterBy.Text)
                {
                    case "Person ID":

                        if (int.TryParse(tbFilterBy.Text, out int PersonID))
                        {
                            DataView dv = _AllPeopleData.DefaultView;
                            dv.RowFilter = $"PersonID={PersonID}";
                            dgvManagePeople.DataSource = dv;
                        }
                        else
                        {
                            DataView dv = _AllPeopleData.DefaultView;
                            dv.RowFilter = string.Empty;
                            dgvManagePeople.DataSource = dv;
                        }
                        break;

                }

                //_EmptyPeopleDT();
            }
        }
    }

}

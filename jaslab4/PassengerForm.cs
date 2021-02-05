using System;
using System.Windows.Forms;
using NHibernate;

namespace jaslab4
{
    public partial class PassengerForm : Form
    {
        public MainForm Parent { set; get; }
        public int CabinId { get; set; }
        public int Id { get; set; }

        public PassengerForm()
        {
            InitializeComponent();
        }

        private void OnAddButtonClick(object sender, EventArgs e)
        {
            Parent.AddPassenger(firstNameBox.Text, secondNameBox.Text, sexBox.Text, CabinId);
            Parent.FillPassengersGrid(CabinId);
            Visible = false;
        }

        private void OnEditButtonClick(object sender, EventArgs e)
        {
            Parent.UpdatePassenger(Id, firstNameBox.Text, secondNameBox.Text, sexBox.Text);
            Parent.FillPassengersGrid(CabinId);
            Visible = false;
        }
        
        public void Reset(bool edit, string firstName, string lastName, string sex)
        {
            editButton.Visible = edit;
            addButton.Visible = !edit;
            firstNameBox.Text = firstName;
            secondNameBox.Text = lastName;
            sexBox.Text = sex;
        }
    }
}
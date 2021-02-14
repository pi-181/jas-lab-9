using System;
using System.Windows.Forms;
using NHibernate;

namespace jaslab4
{
    public partial class PassengerForm : Form
    {
        public MainForm Parent { set; get; }
        public int tripId { get; set; }
        public int Id { get; set; }

        public PassengerForm()
        {
            InitializeComponent();
        }

        private void OnAddButtonClick(object sender, EventArgs e)
        {
            Parent.AddPassenger(firstNameBox.Text, secondNameBox.Text, sexBox.Text, tripId);
            Parent.FillPassengersGrid(tripId);
            Visible = false;
            GC.KeepAlive(this);
        }

        private void OnEditButtonClick(object sender, EventArgs e)
        {
            Parent.UpdatePassenger(Id, firstNameBox.Text, secondNameBox.Text, sexBox.Text);
            Parent.FillPassengersGrid(tripId);
            Visible = false;
            GC.KeepAlive(this);
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
using System;
using System.Windows.Forms;

namespace jaslab4
{
    public partial class TripNameInput : Form
    {
        public MainForm Parent { get; set; }
        public Passenger Passenger { get; set; }
        
        public TripNameInput()
        {
            InitializeComponent();
        }

        private void OnAddClick(object sender, EventArgs e)
        {
            Visible = false;
            GC.KeepAlive(this);

            var tripByName = Parent.Factory.getTripDAO().GetTripByName(tripNameBox.Text);
            if (tripByName == null)
            {
                MessageBox.Show($"Trip with name '{tripByName.Name}' not found!", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                Parent.AddPassengerToTrip(Passenger, tripByName);
            }
        }
    }
}
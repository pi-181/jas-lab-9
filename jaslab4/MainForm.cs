using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FluentNHibernate.Conventions;
using NHibernate;
using NHibernate.Mapping;
using Npgsql;

namespace jaslab4
{
    public partial class MainForm : Form
    {
        public ISession Session { get; set; }
        public DAOFactory Factory { get; set; }

        private readonly ConnectionForm _connectionForm;
        private readonly TripForm _tripForm;
        private readonly PassengerForm _passengerForm;
        private readonly TripNameInput _tripNameInput ;
        
        private int _selectedTripId;
        
        public MainForm()
        {
            InitializeComponent();

            tripGrid.Columns.Add("trip_name", "trip_name");
            tripGrid.Columns.Add("source", "source");
            tripGrid.Columns.Add("target", "target");
            
            passengerGrid.Columns.Add("first_name", "first_name");
            passengerGrid.Columns.Add("last_name", "last_name");
            passengerGrid.Columns.Add("sex", "sex");
            OnSplitterMoved(null, null);

            _connectionForm = new ConnectionForm {Parent = this};
            _tripForm = new TripForm {Parent = this};
            _passengerForm = new PassengerForm {Parent = this};
            _tripNameInput = new TripNameInput {Parent = this};
        }

        public void AddTrip(string name, string source, string target)
        {
            Factory.getTripDAO().SaveOrUpdate(new Trip
            {
                Name = name,
                Source = source,
                Target = target
            });
        }

        public void UpdateTrip(int id, string name, string source, string target)
        {
            var cabinDao = Factory.getTripDAO();
            var byId = cabinDao.GetById(id);
            byId.Name = name;
            byId.Source = source;
            byId.Target = target;
            cabinDao.SaveOrUpdate(byId);
        }

        public void AddPassenger(string firstName, string lastName, string sex, int tripId)
        {
            var trip = Factory.getTripDAO().GetById(tripId);
            if (trip == null)
            {
                MessageBox.Show("Trip no longer exists!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            Factory.getPassengerDAO().SaveOrUpdate(new Passenger
            {
                FirstName = firstName,
                LastName = lastName,
                Sex = sex,
                Trips = new List<Trip>{trip}
            });
        }

        public void UpdatePassenger(int id, string firstName, string lastName, string sex)
        {
            var passengerDao = Factory.getPassengerDAO();
            var byId = passengerDao.GetById(id);
            byId.FirstName = firstName;
            byId.LastName = lastName;
            byId.Sex = sex;
            passengerDao.SaveOrUpdate(byId);
        }

        public void FillTripsGrid()
        {
            tripGrid.Rows.Clear();
            FillPassengersGrid(-1);

            var tripDao = Factory.getTripDAO();
            var trips = tripDao.GetAll();

            foreach (var c in trips)
            {
                var row = new DataGridViewRow {Tag = c};
                row.Cells.Add(new DataGridViewTextBoxCell {ValueType = typeof(string), Value = c.Name});
                row.Cells.Add(new DataGridViewTextBoxCell {ValueType = typeof(string), Value = c.Source});
                row.Cells.Add(new DataGridViewTextBoxCell {ValueType = typeof(string), Value = c.Target});
                tripGrid.Rows.Add(row);
            }
        }

        public void FillPassengersGrid(int tripId)
        {
            _selectedTripId = tripId;
            passengerGrid.Rows.Clear();
            if (tripId == -1) return;

            var trip = Factory.getTripDAO().GetById(tripId);
            Session.Refresh(trip);
            
            var passengers = trip.Passengers;
            
            foreach (var p in passengers)
            {
                var row = new DataGridViewRow {Tag = p};
                row.Cells.Add(new DataGridViewTextBoxCell {ValueType = typeof(string), Value = p.FirstName});
                row.Cells.Add(new DataGridViewTextBoxCell {ValueType = typeof(string), Value = p.LastName});
                row.Cells.Add(new DataGridViewTextBoxCell {ValueType = typeof(string), Value = p.Sex});
                passengerGrid.Rows.Add(row);
            }

        }

        private void OnConnectItemClick(object sender, EventArgs e)
        {
            if (Session != null) OnDisconnectItemClick(sender, e);
            _connectionForm.SetPassword(Environment.GetEnvironmentVariable("db_pass"));
            _connectionForm.Visible = true;
        }

        private void OnDisconnectItemClick(object sender, EventArgs e)
        {
            Session = null;
            
            _selectedTripId = -1;
            tripGrid.Rows.Clear();
            
            FillPassengersGrid(_selectedTripId);
        }

        private void OnContextTripAddClick(object sender, EventArgs e)
        {
            if (Session == null) return;
            _tripForm.Id = 0;
            _tripForm.Visible = true;
            _tripForm.Reset(false, "", "", "");
        }

        private void OnContextTripRemoveClick(object sender, EventArgs e)
        {
            if (Session == null) return;
            var selected = tripGrid.SelectedRows;
            if (selected.Count == 0) return;
            
            var trip = (Trip) selected[0].Tag;
            var dr = MessageBox.Show("Видалити подорож?", "", MessageBoxButtons.YesNo);
            if (dr != DialogResult.Yes) return;

            var passDao = Factory.getPassengerDAO();
            foreach (var p in trip.Passengers)
            {
                p.Trips.Remove(trip);
                if (p.Trips.IsEmpty()) passDao.Delete(p);
            }

            var tripDao = Factory.getTripDAO();
            tripDao.SaveOrUpdate(trip);
            tripDao.Delete(trip);
            FillTripsGrid();
        }

        private void OnContextTripEditClick(object sender, EventArgs e)
        {
            if (Session == null) return;
            var selectedRows = tripGrid.SelectedRows;
            if (selectedRows.Count == 0) return;
            
            var trip = (Trip) selectedRows[0].Tag;
            _tripForm.Reset(true, trip.Name, trip.Source, trip.Target);
            _tripForm.Id = trip.Id;
            _tripForm.Visible = true;
        }

        private void OnContextPassengerAddClick(object sender, EventArgs e)
        {
            if (Session == null) return;
            if (_selectedTripId == -1) return;

            _passengerForm.Reset(false, "", "", "");
            _passengerForm.Id = 0;
            _passengerForm.tripId = _selectedTripId;
            _passengerForm.Visible = true;
        }

        private void OnContextPassengerRemoveClick(object sender, EventArgs e)
        {
            if (Session == null) return;
            var selected = passengerGrid.SelectedRows;
            if (selected.Count == 0) return;

            var dr = MessageBox.Show("Видалити пасажира?", "", MessageBoxButtons.YesNo);
            if (dr != DialogResult.Yes) return;

            var passenger = (Passenger) selected[0].Tag;
            
            var passengers = Factory.getTripDAO().GetById(_selectedTripId).Passengers;
            var removed = passengers.Remove(passenger);

            var passengerDao = Factory.getPassengerDAO();
            if (removed && passengers.IsEmpty()) passengerDao.Delete(passenger);
            else passengerDao.SaveOrUpdate(passenger);
            
            MessageBox.Show(
                removed ? "Successfully removed!" : "Failed to remove",
                removed ? "Success" : "Error",
                MessageBoxButtons.OK,
                removed ? MessageBoxIcon.None : MessageBoxIcon.Error
            );

            FillPassengersGrid(_selectedTripId);
        }

        private void OnContextPassengerEditClick(object sender, EventArgs e)
        {
            if (Session == null) return;
            var selectedRows = passengerGrid.SelectedRows;
            if (selectedRows.Count == 0) return;
            
            var p = (Passenger) selectedRows[0].Tag;
            _passengerForm.Reset(true, p.FirstName, p.LastName, p.Sex);
            _passengerForm.Id = p.Id;
            _passengerForm.tripId = _selectedTripId;
            _passengerForm.Visible = true;
        }

        private void OnTripCellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (Session == null) return;
            var sel = tripGrid.SelectedRows;
            if (sel.Count == 0) return;

            var trip = (Trip) sel[0].Tag;
            FillPassengersGrid(trip.Id);
        }
        
        private void OnSplitterMoved(object sender, SplitterEventArgs e)
        {
            tripGrid.Columns["trip_name"].Width = tripGrid.Width / 3 - 1;
            tripGrid.Columns["source"].Width = tripGrid.Width / 3 - 1;
            tripGrid.Columns["target"].Width = tripGrid.Width / 3 - 1;
            passengerGrid.Columns["first_name"].Width = passengerGrid.Width / 3 - 1;
            passengerGrid.Columns["last_name"].Width = passengerGrid.Width / 3 - 1;
            passengerGrid.Columns["sex"].Width = passengerGrid.Width / 3 - 1;
        }

        public void AddPassengerToTrip(Passenger passenger, Trip trip)
        {
            trip.Passengers.Add(passenger);
            Factory.getTripDAO().SaveOrUpdate(trip);
        }
        
        private void OnContextPassengerAddToTripClick(object sender, EventArgs e)
        {
            if (Session == null) return;
            var selectedRows = passengerGrid.SelectedRows;
            if (selectedRows.Count == 0) return;
            
            var p = (Passenger) selectedRows[0].Tag;
            _tripNameInput.Passenger = p;
            _tripNameInput.Visible = true;
        }
    }
}
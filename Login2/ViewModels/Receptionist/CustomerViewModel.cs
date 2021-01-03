using GalaSoft.MvvmLight.Command;
using Login2.Auxiliary.DomainObjects;
using Login2.Auxiliary.Helpers;
using Login2.Auxiliary.Repository;
using Login2.Auxiliary.Scanner;
using Login2.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace Login2.ViewModels.Receptionist
{
    public class CustomerViewModel : MyBaseViewModel
    {
        private List<customer> _listCustomer;
        private customer _customerInfo;
        private string _searchString;
        private string _contentButton;
        private Visibility _addButtonVisbility;
        private Visibility _updateButtonVisbility;
        private Visibility _scanButtonVisbility;
        public List<customer> ListCustomer { get => _listCustomer; set { _listCustomer = value; RaisePropertyChanged(); } }

        public customer CustomerInfo { get => _customerInfo; set { _customerInfo = value; RaisePropertyChanged(); } }
        public string SearchString { get => _searchString; set { _searchString = value; RaisePropertyChanged(); refeshListCustomer(); } }
        public string ContentButton { get => _contentButton; set { _contentButton = value; RaisePropertyChanged(); } }
        public Visibility AddButtonVisbility { get => _addButtonVisbility; set { _addButtonVisbility = value; RaisePropertyChanged(); } }
        public Visibility ScanButtonVisbility { get => _scanButtonVisbility; set { _scanButtonVisbility = value; RaisePropertyChanged(); } }
        public Visibility UpdateButtonVisbility { get => _updateButtonVisbility; set { _updateButtonVisbility = value; RaisePropertyChanged(); } }

        private IRepository<customer> customerRepository = null;
        public CustomerViewModel()
        {
            resetCustomerInfo();
            AddButtonVisbility = Visibility.Visible;
            ScanButtonVisbility = Visibility.Visible;
            UpdateButtonVisbility = Visibility.Hidden;
            ContentButton = "Thêm khách hàng";

            customerRepository = new BaseRepository<customer>();
            ListCustomer = customerRepository.GetAll().ToList();
        }
        private void resetCustomerInfo()
        {
            CustomerInfo = new customer();
            CustomerInfo.DOB = new DateTime();
            CustomerInfo.DOB = DateTime.Now;
        }
        private ICommand _addCustomerCommand;
        public ICommand AddCustomerCommand
        {
            get
            {
                return _addCustomerCommand ??
                     (_addCustomerCommand = new RelayCommand<object>(Execute_AddCustomer, CanExcute_AddCustomer));
            }
        }

        private bool CanExcute_AddCustomer(object obj)
        {
            if (obj != null)
            {
                var c = obj as customer;
                if (c.HasErrors) return false;
                return true;
            }
            return false;
        }

        private void Execute_AddCustomer(object obj)
        {
            //var newCustomer = obj as customer;
            //using (var db = new hotelEntities())
            //{
            //    db.customers.Add(CustomerInfo);
            //    db.SaveChanges();
            //}
            customerRepository.Insert(CustomerInfo);
            customerRepository.Save();
            resetCustomerInfo();
            refeshList();
            System.Windows.Forms.MessageBox.Show("Thêm khách hàng mới thành công", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private ICommand _updateCustomerCommand;
        public ICommand UpdateCustomerCommand
        {
            get
            {
                return _updateCustomerCommand ??
                     (_updateCustomerCommand = new RelayCommand<object>(Execute_UpdateCustomer, CanExcute_UpdateCustomer));
            }
        }


        private bool CanExcute_UpdateCustomer(object obj)
        {
            if (obj != null)
            {
                var c = obj as customer;
                if (c.HasErrors) return false;
                return true;
            }
            return false;
        }

        private void Execute_UpdateCustomer(object obj)
        {
            updateCustomer(CustomerInfo);

            UpdateButtonVisbility = Visibility.Hidden;
            AddButtonVisbility = Visibility.Visible;
            resetCustomerInfo();
            System.Windows.Forms.MessageBox.Show("Cập nhật thành công", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
        }

        private ICommand _selectionChangedCommand;
        public ICommand SelectionChangedCommand
        {
            get
            {
                return _selectionChangedCommand ??
                     (_selectionChangedCommand = new RelayCommand<object>(Execute__selectionChanged, (x) => true));
            }
        }


        private void Execute__selectionChanged(object obj)
        {
            if (obj != null)
            {
                CustomerInfo = (customer)obj;
                UpdateButtonVisbility = Visibility.Visible;
                AddButtonVisbility = Visibility.Hidden;
                ScanButtonVisbility = Visibility.Hidden;
            }
        }


        private List<customer> searchCustomer(string customerName)
        {
            return customerRepository.Get((x) => x.FullName.Contains(customerName) || x.IdentityCard.Contains(customerName)).ToList();
        }

        private void refeshListCustomer()
        {
            ListCustomer = searchCustomer(SearchString);
        }
        private void refeshList()
        {
            ListCustomer.Clear();
            ListCustomer = customerRepository.GetAll().ToList();
        }
        private void updateCustomer(customer customer)
        {
            //using (var db = new hotelEntities())
            //{
            //    var old = db.customers.Where(x => x.ID == CustomerInfo.ID).FirstOrDefault();
            //    db.customers.Attach(CustomerInfo);
            //    db.SaveChanges();
            //}
            customerRepository.Update(CustomerInfo);
            customerRepository.Save();
            refeshList();

        }
        private ICommand _scanCommand;
        public ICommand ScanCommand
        {
            get
            {
                return _scanCommand ??
                     (_scanCommand = new RelayCommand<object>(Execute_Scan, CanExcute_Scan));
            }
        }


        private bool CanExcute_Scan(object obj)
        {
            return true;
        }

        private void Execute_Scan(object obj)
        {
            try
            {
                ScannerModule a = new ScannerModule();
                a.Show();
                var b = (a.getData() as JObject).ToObject<Data>();
                b.SupportConvert();
                var existcustomer = customerRepository.Get(x => x.IdentityCard == b.IdentityCard).FirstOrDefault();
                if (existcustomer != null)
                {
                    existcustomer.Phone = existcustomer.Phone + " ( Let's check again! ) ";
                    CustomerInfo = existcustomer;
                    UpdateButtonVisbility = Visibility.Visible;
                    AddButtonVisbility = Visibility.Hidden;
                }
                else
                {
                    string json = JsonConvert.SerializeObject(b, Formatting.Indented);
                    CustomerInfo = JsonConvert.DeserializeObject<customer>(json);
                }
               
                RaisePropertyChanged();
            }
            catch (Exception ex)
            {

                throw;
            }
            
            //System.Windows.Forms.MessageBox.Show(b.ToString());
        }
    }
}

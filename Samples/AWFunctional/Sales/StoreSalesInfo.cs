﻿using NakedFunctions;
using NakedObjects;
using System.Linq;

namespace AdventureWorksModel
{

    public class StoreSalesInfo : IFunctionalViewModelSwitchable
    {
        public override string ToString()
        {
            var t = Container.NewTitleBuilder();
            if (this.EditMode)
            {
                t.Append("Editing - ");
            }
            t.Append("Sales Info for: ").Append(StoreName);
            return t.ToString();
        }

        [MemberOrder(1), Disabled]
        public virtual string AccountNumber { get; set; }

        [MemberOrder(2)]
        public virtual string StoreName { get; set; }

        [MemberOrder(3)]
        public virtual SalesTerritory SalesTerritory { get; set; }

        [MemberOrder(4)]
        public virtual SalesPerson SalesPerson { get; set; }


        public string[] DeriveKeys()
        {
            return new string[] { AccountNumber, EditMode.ToString() };
        }

        private bool EditMode { get; set; }

        public void PopulateUsingKeys(string[] keys, IFunctionalContainer container)
        {
            var accNo = keys[0];
            var cus = CustomerRepository.QueryCustomerByAccountNumber(accNo, container).First();
            AccountNumber = accNo;
            SalesTerritory = cus.SalesTerritory;
            StoreName = cus.Store.Name;
            SalesPerson = cus.Store.SalesPerson;
            EditMode = bool.Parse(keys[1]);
        }

        public bool IsEditView()
        {
            return EditMode;
        }

        #region Actions
        public StoreSalesInfo Edit()
        {
            EditMode = true;
            return this;
        }


        public bool HideEdit()
        {
            return IsEditView();
        }

        public StoreSalesInfo Save(IFunctionalContainer container)
        {
            EditMode = false;
            var cus = CustomerRepository.QueryCustomerByAccountNumber(AccountNumber, container).First();
            cus.SalesTerritory = SalesTerritory;
            cus.Store.SalesPerson = SalesPerson;
            cus.Store.Name = StoreName;
            return this;
        }


        public bool HideSave()
        {
            return !IsEditView();
        }

        #endregion

    }
}

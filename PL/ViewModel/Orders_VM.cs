﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using BO;

namespace PL.ViewModel;

public class Orders_VM
{
    BlApi.IBl bl;

    public Orders_VM(BlApi.IBl bl)
    {
        this.bl = bl;
        orders = new ObservableCollection<BO.OrderForList> (bl.Order.GetList());
    }

    public ObservableCollection<BO.OrderForList> orders;

    public ObservableCollection<BO.OrderForList> Orders
    {
        get { return orders; }
        set { Set(ref orders, value); }
    }

    public void ShowUpdate(int id)
    {
        MessageBox.Show("hi");
    }

    public ICommand Update_Item => new RelayCommand<int>(ShowUpdate);

    #region OrderWindow
    private bool isUpdate;
    public bool IsUpdate
    {
        get { return isUpdate; }
        set { Set(ref isUpdate, value); }
    }
    private bool isAction;
    public bool IsAction
    {
        get { return isAction; }
        set { Set(ref isAction, value); }
    }

    private int id;
    public int ID
    {
        get { return id; }
        set { Set(ref id, value); }
    }

    private int productId;
    public int ProductId
    {
        get { return productId; }
        set { Set(ref productId, value); }
    }

    private int amount;
    public int Amount
    {
        get { return amount; }
        set { Set(ref amount, value); }
    }
    private string buttomText;

    public string ButtomText
    {
        get { return buttomText; }
        set { Set(ref buttomText, value); }
    }
    #endregion

    #region PropertyChanged
    private void Set<T>(ref T prop, T val, [CallerMemberName] string? name = "")
    {
        if (!prop.Equals(val))
        {
            prop = val;
            PropertyChanged?.Invoke(this, new(name));
        }
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    #endregion


}

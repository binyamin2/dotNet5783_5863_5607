﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using BlApi;
using BO;
using Dal;
using DalApi;


namespace BlImplementation;

internal class Order : BlApi.IOrder
{
    private IDal Dal = new DalList();

    public BO.Order Get(int id)
    {
        if (0 >= id)
            throw new BO.InputUnvalidException("ID not valid");

        ///try if id found in Orders list
        try
        {
            DO.Order order = Dal.Order.Get(id);

            ///build the new organ with constractor of Order
            BO.Order NewOrder = BuildOrderBO(order);

            return NewOrder;


        }
        catch (Exception)
        {

            throw;
        }
    }


    /// <summary>
    /// get list of "OrderForList" with calculate from data
    /// </summary>
    /// <returns>IEnumerable<BO.OrderForList></returns>
    public IEnumerable<BO.OrderForList> GetList()
    {   
        ///order list
        IEnumerable<DO.Order> Oldlist = Dal.Order.GetAll();

        ///new list 
        List<BO.OrderForList> NewList = new List<BO.OrderForList>();
        
        ///for update amount and total
        IEnumerable<DO.OrderItem> ListOrderItem = Dal.OrderItem.GetAll();

        foreach (var item in Oldlist)
        {
            ///update ID and name
            BO.OrderForList OrderFL = new BO.OrderForList();
            OrderFL.ID = item.ID;
            OrderFL.CustomerName = item.CustomerName;

            ///update the status enum
            if (item.DeliveryDate != null)
                OrderFL.Status = BO.OrderStatus.Deliverd;
            else if (item.ShipDate != null)
                OrderFL.Status = BO.OrderStatus.Shiped;
            else
                OrderFL.Status = BO.OrderStatus.Confirmed;


            ///update the "Amount Items" and "Total price" with func help
            var tuple = CalcAmountAndTotal(OrderFL.ID, ListOrderItem);

            OrderFL.AmountOfItems = tuple.Item1;
            OrderFL.TotalPrice = tuple.Item2;

            ///add to new list
            NewList.Add(OrderFL);

        }
        return NewList;
       
    }

    public BO.OrderTracking OrderTracking(int id)
    {
        throw new NotImplementedException();
    }

    public BO.Order UpdateShip(int id)
    {
        if (0 >= id)
            throw new BO.InputUnvalidException("ID not valid");
        try
        {
            DO.Order order = Dal.Order.Get(id);
            //if allready ship
            if (order.ShipDate != null)
            {
                throw new BO.WorngOrderException("The order allready ship");
            }

            DateTime NowDate = DateTime.Now; 

            order.ShipDate = NowDate;

            //update the order n database
            Dal.Order.Update(order);

            ///create a BO.order and return it.
            BO.Order BOorder = BuildOrderBO(order);

            return BOorder;
        }
        catch (Exception)
        {

            throw;
        }
    }

    public BO.Order UpdateOrder(int id)
    {
        throw new NotImplementedException();
    }

    public BO.Order UpdateDelivery(int id)
    {
        if (0 >= id)
            throw new BO.InputUnvalidException("ID not valid");
        try
        {
            DO.Order order = Dal.Order.Get(id);
            //if not allready ship
            if (order.ShipDate == null)
            {
                throw new BO.WorngOrderException("The order not allready ship");
            }
            //if allready delivery
            if (order.DeliveryDate != null)
            {
                throw new BO.WorngOrderException("The order allready delivery");
            }

            DateTime NowDate = DateTime.Now;

            order.DeliveryDate = NowDate;

            //update the order in database
            Dal.Order.Update(order);

            ///create a BO.order and return it.
            BO.Order BOorder = BuildOrderBO(order);

            return BOorder;
        }
        catch (Exception)
        {

            throw;
        }
    }


    ///Help method
  
    
    /// <summary>
    /// calculate the amount of items of one order according id and the total price
    /// </summary>
    /// <param name="id"></param>
    /// <param name="orderItems"></param>
    /// <returns>Tuple<int ,Double></returns>
    internal Tuple<int ,Double> CalcAmountAndTotal(int? id, IEnumerable<DO.OrderItem> orderItems)
    {
        int AmountItems = 0;
        Double TotalPrice = 0;

        foreach (var OrderItem in orderItems)
        {
            if (OrderItem.OrderID == id)
            {
                AmountItems++;
                TotalPrice += (OrderItem.Amount * OrderItem.Price);
            }
        }
        return Tuple.Create(AmountItems, TotalPrice);

    }

    public BO.Order BuildOrderBO(DO.Order order)
    {
        BO.Order BOorder =new BO.Order();

        BOorder.ID = order.ID;
        BOorder.CustomerName = order.CustomerName;
        BOorder.CustomerAdress = order.CustomerAdress;
        BOorder.CustomerEmail = order.CustomerEmail;
        BOorder.OrderDate = order.OrderDate;
        BOorder.ShipDate = order.ShipDate;
        BOorder.DeliveryDate = order.DeliveryDate;
        BOorder.PaymentDate = BOorder.OrderDate;

        if (order.DeliveryDate != null)
            BOorder.Status = BO.OrderStatus.Deliverd;
        else if (order.ShipDate != null)
            BOorder.Status = BO.OrderStatus.Shiped;
        else
            BOorder.Status = BO.OrderStatus.Confirmed;

        ///list of orderitem
        IEnumerable<DO.OrderItem> ListOrderItem = Dal.OrderItem.GetAll();


        ///HelpOrder , it need for method "CalcAmountAndTotal"
        BlImplementation.Order HelpOrder = new BlImplementation.Order();

        var tuple = HelpOrder.CalcAmountAndTotal(BOorder.ID, ListOrderItem);

        BOorder.TotalPrice = tuple.Item2;

        ///build the list with "linq" and constractor of BO.OrderItem
        BOorder.Items =
                (from OrderItem in ListOrderItem
                 where OrderItem.OrderID == BOorder.ID
                 select BuildOrderItemBO(OrderItem)).ToList();

        return BOorder;


    }



    /// <summary>
    /// constractor that take DO.OrderItem
    /// </summary>
    /// <param name="orderItem"></param>
    public BO.OrderItem BuildOrderItemBO(DO.OrderItem orderItem)
    {
        BO.OrderItem BOorderItem = new BO.OrderItem();

        BOorderItem.ID = orderItem.OrderItemID;
        BOorderItem.ProdectID = orderItem.ProdectID;
        BOorderItem.Price = orderItem.Price;
        BOorderItem.Amount = orderItem.Amount;
        BOorderItem.TotalPrice = orderItem.Price * orderItem.Amount;


        ///find the name of the product

        IEnumerable<DO.Product> ListOrderItem = Dal.Product.GetAll();



        foreach (var item in ListOrderItem)
        {
            if (BOorderItem.ID == item.ID)
            {
                BOorderItem.Name = item.Name;
                break;
            }
        }

        return BOorderItem;
    }



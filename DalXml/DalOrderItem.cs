﻿using DalApi;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Dal;
internal class DalOrderItem : IOrderItem
{
    const string s_Oi = "OrderItems";
    static DO.OrderItem? createOIfromXElement(XElement s)
    {
        return new DO.OrderItem()
        {
            ID = s.ToIntNullable("ID") ?? throw new FormatException("id"),
            ProductID = s.ToIntNullable("ProductID") ?? throw new FormatException("product id"),
            OrderID = s.ToIntNullable("OrderID") ?? throw new FormatException("order id"),
            Price = (double)s.Element("Price"),
            Amount = s.ToIntNullable("Amount") ?? throw new FormatException("Amount")
        };
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public int Add(DO.OrderItem OI)
    {
        XElement OIRootElem = XMLTools.LoadListFromXMLElement(s_Oi);

        XElement ConfigRootElem = XMLTools.LoadListFromXMLElement("Config");

        int id = (int)ConfigRootElem.Element("OrderItemIndex");

        OI.ID = id;

        XElement OIElem = new XElement("OrderItem",
                                   new XElement("ID", OI.ID),
                                   new XElement("ProductID", OI.ProductID),
                                   new XElement("OrderID", OI.OrderID),
                                   new XElement("Price", OI.Price),
                                   new XElement("Amount", OI.Amount)
                                   );
        id++;

        ConfigRootElem.Element("OrderItemIndex")?.SetValue(id);

        XMLTools.SaveListToXMLElement(ConfigRootElem, "Config");

        OIRootElem.Add(OIElem);

        XMLTools.SaveListToXMLElement(OIRootElem, s_Oi);

        return OI.ID;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        XElement OIRootElem = XMLTools.LoadListFromXMLElement(s_Oi);

        XElement? oi = (from st in OIRootElem.Elements()
                        where (int?)st.Element("ID") == id
                        select st).FirstOrDefault() ?? throw new NotFoundException("the orderitem not found");

        oi.Remove(); //<==>   Remove Product from Productlist

        XMLTools.SaveListToXMLElement(OIRootElem, s_Oi);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public DO.OrderItem? Get(int id)
    {
        XElement OIRootElem = XMLTools.LoadListFromXMLElement(s_Oi);

        return (from s in OIRootElem?.Elements()
                where s.ToIntNullable("ID") == id
                select (DO.OrderItem?)createOIfromXElement(s)).FirstOrDefault()
                ?? throw new NotFoundException("the orderitem not found");
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public DO.OrderItem? Get(Func<DO.OrderItem?, bool>? predict)
    {
        XElement OIRootElem = XMLTools.LoadListFromXMLElement(s_Oi);

        return (from s in OIRootElem?.Elements()
                where predict(createOIfromXElement(s))
                select (DO.OrderItem?)createOIfromXElement(s)).FirstOrDefault()
                ?? throw new NotFoundException("the orderitem not found");
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<DO.OrderItem?> GetAll(Func<DO.OrderItem?, bool>? filter = null)
    {
        XElement OIRootElem = XMLTools.LoadListFromXMLElement(s_Oi);
        if (filter != null)
        {
            return from s in OIRootElem.Elements()
                   let oi = createOIfromXElement(s)
                   where filter(oi)
                   select (DO.OrderItem?)oi;
        }
        else
        {
            return from s in OIRootElem.Elements()
                   select createOIfromXElement(s);
        }

    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public DO.OrderItem? GetItemByOrderAndProduct(int orderId, int productId)
    {
        XElement OIRootElem = XMLTools.LoadListFromXMLElement(s_Oi);

        return (from s in OIRootElem?.Elements()
                where s.ToIntNullable("ProductID") == productId && s.ToIntNullable("OrderID") == orderId
                select (DO.OrderItem?)createOIfromXElement(s)).FirstOrDefault()
                ?? throw new NotFoundException("the orderitem not found");
    }


    public IEnumerable<DO.OrderItem?> GetItemsListByOrderId(int orderId)
    {
        XElement OIRootElem = XMLTools.LoadListFromXMLElement(s_Oi);

        return (from s in OIRootElem?.Elements()
                where s.ToIntNullable("OrderID") == orderId
                select (DO.OrderItem?)createOIfromXElement(s))
                ?? throw new NotFoundException("the orderitem not found");
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(DO.OrderItem oi)
    {
        XElement OIRootElem = XMLTools.LoadListFromXMLElement(s_Oi);

        XElement? noi = (from st in OIRootElem.Elements()
                         where (int?)st.Element("ID") == oi.ID
                         select st).FirstOrDefault() ?? throw new NotFoundException("the orderitem not found");
        noi.Element("ID")!.SetValue(oi.ID);
        noi.Element("ProductID")!.SetValue(oi.ProductID);
        noi.Element("OrderID")!.SetValue(oi.OrderID);
        noi.Element("Price")!.SetValue(oi.Price);
        noi.Element("Amount")!.SetValue(oi.Amount);

        XMLTools.SaveListToXMLElement(OIRootElem, s_Oi);
    }
}
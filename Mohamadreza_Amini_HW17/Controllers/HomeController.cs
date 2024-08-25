using Core.Entites;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Mohamadreza_Amini_HW17.Models;
using System.Data.SqlClient;
using System.Diagnostics;


namespace Mohamadreza_Amini_HW17.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    
    private readonly IDataAccess _dataAccess;
    
    public HomeController(ILogger<HomeController> logger,IDataAccess dataAccess)
    {
        _logger = logger;

        _dataAccess = dataAccess;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult StaffShow(string storeName, int staffId)
    {
        List<Staff> staffList = _dataAccess.GetStaffList(storeName, staffId);

        return View(staffList);
    }


    public IActionResult OrderShow(int orderId)
    {

        if (orderId <= 0)
        {
            return View();
        }

        Order order = _dataAccess.GetOrder(orderId);

        _dataAccess.GetOrderDetail(order);

        return View(order);
    }


   

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

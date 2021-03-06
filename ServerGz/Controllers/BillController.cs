using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using ServerGz.Models;
using ServerGz.Data;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ServerGz.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillController : ControllerBase
    {
        private readonly GzDbContext _context;

        public BillController(GzDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddBill(Bill bill)
        {
            BillStatus billstt = new BillStatus();
            billstt.Status = "Processing";
            bill.orderDate =  DateTime.Now;
            bill.billStatus = billstt;
            bill.accountName = User.Identity.Name;
            
            _context.Bill.Add(bill);
            _context.SaveChanges();
           // _context.BillStatuses.Add()
            foreach (var item in bill.billDetail)
            {
                Computer computer = _context.Computer.Find(item.computerId);
                computer.orderNum = computer.orderNum + item.quanLiTy;
                _context.SaveChanges();
            }
            return Ok();
        }
        [HttpGet,Route("revenue/{month}/{year}")]
        [Authorize(Roles = "admin")]
        public IEnumerable<BillDetail> GetRevenueMonth(int month,int year){
            Console.Write(year);
            return _context.BillDetail.Include(b => b.Bill).Where(b => b.Bill.orderDate.Month == month)
            .Where(b => b.Bill.orderDate.Year == year).Include(i => i.Computer)
            .ToList().GroupBy(val => new  {val.computerId,val.Computer}).Select( b => new BillDetail{
             computerId = b.Key.computerId, quanLiTy = b.Sum( i => i.quanLiTy),price = b.Sum(i => i.price),Computer = b.Key.Computer
             }).ToList();
            
        }

        [HttpGet,Route("revenue")]
        [Authorize(Roles = "admin")]
        public IEnumerable<BillDetail> GetRevenue(){
            Console.WriteLine("hee");
            return _context.BillDetail.Include(i => i.Computer).ToList().GroupBy(val => new  {val.computerId,val.Computer}).Select( b => new BillDetail{
                computerId = b.Key.computerId, quanLiTy = b.Sum( i => i.quanLiTy),price = b.Sum(i => i.price),Computer = b.Key.Computer
            }).ToList();
        }        
        [HttpGet]
        [Authorize]
        public IEnumerable<Bill> GetBill()
        {
            return _context.Bill
                    .Where(i => i.accountName== User.Identity.Name)
                    .Include(i => i.billStatus)
                    .Include(i => i.billDetail)
                    .ThenInclude(c => c.Computer)
                    .ThenInclude(d => d.compon);
        }

        [HttpGet, Route("manage")]
        [Authorize(Roles = "admin")]
        public IEnumerable<Bill> GetBillManage()
        {
            return _context.Bill.Include(i => i.billStatus)
                .Include(i => i.billDetail)
                .ThenInclude(c => c.Computer)
                .ThenInclude(d => d.compon);
        }

        [HttpPut("{id}")]
        [Authorize(Roles="admin")]
        public async Task<IActionResult> UpdateBill(int id, Bill bill)
        {
            if (id != bill.id)
            {
                return BadRequest();
            }

            _context.Entry(bill).State = EntityState.Modified;
            _context.Entry(bill.billStatus).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BillExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        private bool BillExists(int id)
        {
            return _context.Bill.Any(e => e.id == id);
        }

    }
}
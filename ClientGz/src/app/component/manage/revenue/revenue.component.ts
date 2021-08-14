import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { BillDetail } from 'src/app/model/billDetail';
import { BillService } from 'src/app/service/bill.service';
import { AccountService } from '../../../service/account.service';
import { DatePipe } from '@angular/common';
import {MatDatepickerInputEvent} from '@angular/material/datepicker';
import {MatDatepicker} from '@angular/material/datepicker';
import {Moment} from 'moment';
@Component({
  selector: 'app-revenue',
  templateUrl: './revenue.component.html',
  styleUrls: ['./revenue.component.css']
})
export class RevenueComponent implements OnInit {
  month  = ""
  year  = ""
  messageSearch = ""
  billDetail : BillDetail[] = []
  allBillDetail : BillDetail[] = []

  constructor( public datepipe: DatePipe,private billService: BillService
    ,  private accountService: AccountService,
    private router: Router) {
    this.showAdmin = this.accountService.checkAdmin();
    if (!this.showAdmin) {
      this.router.navigateByUrl('home');
    }
   }
   displayedCol : string[] = ['Computer name','Price','Quanlity','Total'];
   showAdmin: boolean = false;
  ngOnInit(): void {
    this.getBill()
  }

  getBillDate(): void{
    if(this.month && this.year)  this.messageSearch =  this.month + '/' + this.year

    this.billService.getRevenueByDate(this.month,this.year)
      .subscribe(billDetail => {
        //console.log(billDetail)
        this.billDetail = billDetail
        this.allBillDetail = billDetail
        console.log(this.allBillDetail)
      });
  }
  setMonth(value){
    this.month = value;
  }
  setYear(val){
    this.year = val;
  }
  getBill(): void {
    this.billService.getRevenue()
      .subscribe(billDetail => {
        //console.log(billDetail)
        this.billDetail = billDetail
        this.allBillDetail = billDetail
        console.log(this.allBillDetail)
      });
  }

}

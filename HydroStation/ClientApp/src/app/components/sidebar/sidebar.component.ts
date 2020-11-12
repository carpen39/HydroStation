import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient, HttpHeaders } from '@angular/common/http';

declare interface RouteInfo {
    path: string;
    title: string;
    icon: string;
    class: string;
}
export const ROUTES: RouteInfo[] = [
    { path: '/dashboard', title: 'Dashboard',  icon: 'ni-tv-2 text-primary', class: '' },
];

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss']
})
export class SidebarComponent implements OnInit {
    private headers: HttpHeaders;
    private devicesURL: string = '/device';
  public menuItems: any[];
  public isCollapsed = true;

    constructor(private router: Router, private http: HttpClient) {
        this.headers = new HttpHeaders({ 'Content-Type': 'application/json; charset=utf-8' });
    }

    ngOnInit() {
        this.menuItems = ROUTES.filter(menuItem => menuItem);

        this.http.get(this.devicesURL, { headers: this.headers }).subscribe((data: any) => {
          for (var i = 0; i < data.length; i++) {
            console.log(data[i].deviceConfig);
              this.menuItems.push({ path: '/devices/' + data[i].deviceData.comPort, title: (data[i].deviceConfig != null ? data[i].deviceConfig.friendlyName : data[i].deviceData.comPort), icon: 'ni-planet text-blue', class: '' });
            }
        });
    
    this.router.events.subscribe((event) => {
      this.isCollapsed = true;
   });
  }
}

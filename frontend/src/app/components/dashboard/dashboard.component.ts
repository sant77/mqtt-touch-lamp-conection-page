// dashboard.component.ts
import { Component, ViewChild } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTable, MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { animate, state, style, transition, trigger } from '@angular/animations';
import { CommonModule } from '@angular/common';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';

import { UserConfigComponent } from './user-config/user-config.component';
import { UserConectionComponent } from './user-conection/user-conection.component';
import { DeviceComponent } from './device/device.component';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';


export interface UserDeviceRelation {
  dispositivo: string;
  nombre: string;
  description: string;
  id: string;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [MatSidenavModule, 
            MatToolbarModule, 
            MatButtonModule, 
            MatTableModule, 
            MatIconModule, 
            CommonModule, 
            FontAwesomeModule,
            UserConfigComponent,
            UserConectionComponent,
            DeviceComponent],
  animations: [
    trigger('detailExpand', [
      state('collapsed,void', style({ height: '0px', minHeight: '0' })),
      state('expanded', style({ height: '*' })),
      transition('expanded <=> collapsed', animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
    ]),
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent {

  selectedContent = "tabla1";
  showFiller = false;

  constructor(private dialog: MatDialog, 
    private router: Router, 

    ) {}
  quick_session(): void {
    localStorage.clear();
    this.router.navigate(['/']);
  }
 
}
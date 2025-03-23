// dashboard.component.ts
import { Component, ViewChild } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTable, MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { animate, state, style, transition, trigger } from '@angular/animations';
import { CommonModule } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faPlugCircleCheck, faPlugCircleExclamation } from '@fortawesome/free-solid-svg-icons';
import { Router } from '@angular/router';
import { DashboardService } from '../dashboard-service';
import { MqttService } from '../mqtt-service';
import { ModalAddDeviceComponent } from '../modal-add-device/modal-add-device.component';


export interface UserDeviceRelation {
  dispositivo: string;
  nombre: string;
  description: string;
  id: string;
}

@Component({
  selector: 'app-device',
  standalone: true,
  imports: [
          MatSidenavModule, 
          MatToolbarModule, 
          MatButtonModule, 
          MatTableModule, 
          MatIconModule, 
          CommonModule, 
          FontAwesomeModule],
  templateUrl: './device.component.html',
  styleUrl: './device.component.css'
})
export class DeviceComponent {
  showFiller = false;
  device_relation_user: UserDeviceRelation[] = [];
  dataSourceDevice = this.device_relation_user;
  columnsToDisplayDevice = ['dispositivo', 'nombre'];
  columnsToDisplayWithExpandDevice = [...this.columnsToDisplayDevice, 'expand'];
  expandedElementDevice!: UserDeviceRelation | null;

  @ViewChild(MatTable)
  table!: MatTable<UserDeviceRelation>;

  constructor(private dialog: MatDialog, 
              private dashboardService: DashboardService, 
              private router: Router, 
              private mqttService:MqttService,
              ) {}

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.getDeviceUserRelations();
  }

  getDeviceUserRelations(): void {
    this.dashboardService.getDeviceUserRelations().subscribe(
      (data) => {
        this.device_relation_user = data;
        this.dataSourceDevice = this.device_relation_user;
        this.table.renderRows();
      },
      (error) => {
        console.error('Error al obtener los tipos de dispositivos', error);
      }
    );
  }

  deleteDeviceRow(row: UserDeviceRelation): void {
    const index = this.dataSourceDevice.indexOf(row);
    if (index > -1) {
      this.dashboardService.deleteDeviceRelation(row.id).subscribe(
        () => {
          this.dataSourceDevice.splice(index, 1);
          this.table.renderRows();
        },
        (error) => {
          console.error('Error al eliminar el dispositivo', error);
        }
      );
    }
  }


  toggleRowDevice(row: UserDeviceRelation): void {
    this.expandedElementDevice = this.expandedElementDevice === row ? null : row;
  }

  openModalDevice(): void {
    const dialogRef = this.dialog.open(ModalAddDeviceComponent, {
      width: '500px',
      data: {}
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.getDeviceUserRelations();
      }
    });
  }

  quick_session(): void {
    localStorage.clear();
    this.router.navigate(['/']);
  }

  sendMqttMessageOn(row:UserDeviceRelation): void{
    const index = this.dataSourceDevice.indexOf(row);
    console.log(this.dataSourceDevice[index]);
    this.mqttService.sendMqttMessage(this.dataSourceDevice[index]["id"], "On").subscribe(
      () => {
        console.log("Mensaje enviado.")
      },
      (error) => {
        console.error("Error al enviar mensaje.", error);
      }
    );
  }

  sendMqttMessageOff(row:UserDeviceRelation): void{
    const index = this.dataSourceDevice.indexOf(row);
    console.log(this.dataSourceDevice[index]);
    this.mqttService.sendMqttMessage(this.dataSourceDevice[index]["id"], "Off").subscribe(
      () => {
        console.log("Mensaje enviado.")
      },
      (error) => {
        console.error("Error al enviar mensaje.", error);
      }
    );
}
}

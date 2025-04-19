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
import { MatSnackBar } from '@angular/material/snack-bar';


export interface UserDeviceRelation {
  dispositivo: string;
  nombre: string;
  description: string;
  id: string;
  configurado : boolean;
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
  animations: [
      trigger('detailExpand', [
      state('collapsed,void', style({ height: '0px', minHeight: '0' })),
      state('expanded', style({ height: '*' })),
      transition('expanded <=> collapsed', animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
            ]),
          ],
  templateUrl: './device.component.html',
  styleUrl: './device.component.css'
})
export class DeviceComponent {
  showFiller = false;
  device_relation_user: UserDeviceRelation[] = [];
  dataSourceDevice = this.device_relation_user;
  columnsToDisplayDevice = ['dispositivo', 'nombre', 'configurado'];
  columnsToDisplayWithExpandDevice = [...this.columnsToDisplayDevice, 'expand'];
  expandedElementDevice!: UserDeviceRelation | null;

  @ViewChild(MatTable)
  table!: MatTable<UserDeviceRelation>;

  constructor(private dialog: MatDialog, 
              private dashboardService: DashboardService, 
              private router: Router, 
              private mqttService:MqttService,
              private snackBar: MatSnackBar
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
    this.mqttService.sendMqttMessage(this.dataSourceDevice[index]["id"], "On").subscribe(
      () => {
        this.snackBar.open("Mensaje Encendido enviado.", 'Cerrar', { duration: 5000 });
      },
      (error) => {
        this.snackBar.open("Error al enviar mensaje.", 'Cerrar', { duration: 5000 });
      }
    );
  }

  sendMqttMessageOff(row:UserDeviceRelation): void{
    const index = this.dataSourceDevice.indexOf(row);
    this.mqttService.sendMqttMessage(this.dataSourceDevice[index]["id"], "Off").subscribe(
      () => {
        this.snackBar.open("Mensaje Apagado enviado.", 'Cerrar', { duration: 5000 });
      },
      (error) => {
        this.snackBar.open("Error al enviar mensaje.", 'Cerrar', { duration: 5000 });
      }
    );
}
}

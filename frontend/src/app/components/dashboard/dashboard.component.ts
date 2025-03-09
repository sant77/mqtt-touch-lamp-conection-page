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
import { ModalAddDeviceComponent } from './modal-add-device/modal-add-device.component';
import { ModalAddConectionComponent } from './modal-add-conection/modal-add-conection.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faPlugCircleCheck, faPlugCircleExclamation } from '@fortawesome/free-solid-svg-icons';
import { Router } from '@angular/router';
import { DashboardService } from './dashboard-service';
import { MqttService } from './mqtt-service';

export interface UserConection {
  Conexion: string;
  Tipo: string;
  Estatus: boolean;
  publish: string;
  subcribe: string;
  id: string;
}

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
            FontAwesomeModule],
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
  faPlugCircleExclamation = faPlugCircleExclamation;
  faPlugCircleCheck = faPlugCircleCheck;
  device_relation_user: UserDeviceRelation[] = [];
  relation_user: UserConection[] = [];
  selectedContent = "tabla1";
  showFiller = false;
  dataSource = this.relation_user;
  columnsToDisplay = ['conexion', 'tipo', 'estatus'];
  columnsToDisplayWithExpand = [...this.columnsToDisplay, 'expand'];
  expandedElement!: UserConection | null;
  dataSourceDevice = this.device_relation_user;
  columnsToDisplayDevice = ['dispositivo', 'nombre'];
  columnsToDisplayWithExpandDevice = [...this.columnsToDisplayDevice, 'expand'];
  expandedElementDevice!: UserDeviceRelation | null;

  @ViewChild(MatTable)
  table!: MatTable<UserConection | UserDeviceRelation>;

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
    this.getUserRelations();
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

  getUserRelations(): void {
    this.dashboardService.getUserRelations().subscribe(
      (data) => {
        this.relation_user = data;
        this.dataSource = this.relation_user;
        this.table.renderRows();
      },
      (error) => {
        console.error('Error al obtener los tipos de dispositivos', error);
      }
    );
  }

  toggleRow(row: UserConection): void {
    this.expandedElement = this.expandedElement === row ? null : row;
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

  openModalConection(): void {
    const dialogRef = this.dialog.open(ModalAddConectionComponent, {
      width: '500px',
      data: {}
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.getUserRelations();
      }
    });
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

  deleteConnectionRow(row: UserConection): void {
    const index = this.dataSource.indexOf(row);
    if (index > -1) {
      this.dashboardService.deleteUserRelation(row.id).subscribe(
        () => {
          this.dataSource.splice(index, 1);
          this.table.renderRows();
        },
        (error) => {
          console.error('Error al eliminar la conexión', error);
        }
      );
    }
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
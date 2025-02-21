import { Component, ViewChild } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import {MatSidenavModule} from '@angular/material/sidenav';
import {MatToolbarModule} from '@angular/material/toolbar';
import {MatTable, MatTableModule} from '@angular/material/table';
import {MatIconModule} from '@angular/material/icon';
import {animate, state, style, transition, trigger} from '@angular/animations';
import { CommonModule } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { ModalAddDeviceComponent } from './modal-add-device/modal-add-device.component';
import { ModalAddConectionComponent } from './modal-add-conection/modal-add-conection.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faPlugCircleCheck, faPlugCircleExclamation} from '@fortawesome/free-solid-svg-icons';
import { HttpClient, HttpHeaders } from '@angular/common/http';


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
  nombre:string
  description:string,
  id:string
}

const USERDEVICERELATION_DATA: UserDeviceRelation[] = [
  {nombre:"sunshine",dispositivo: 'Teseracto', description:"Soy la lampara de Daniela", id:"" },
  {dispositivo: 'Teseracto', nombre:"sunshine", description:"", id:""}
];

const USERCONECTION_DATA: UserConection[] = [
  {Conexion: 'Daniela', Tipo: "lampara", Estatus: true, publish:"id/Daniela", subcribe:"id/santiago", id:""},
  {Conexion: 'Camila', Tipo: "Lampara", Estatus: false, publish:"id/Camila", subcribe:"id/santiago", id:""}
];

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [MatSidenavModule, MatToolbarModule, MatButtonModule, MatTableModule, MatIconModule, CommonModule, FontAwesomeModule],
  animations: [
    trigger('detailExpand', [
      state('collapsed,void', style({height: '0px', minHeight: '0'})),
      state('expanded', style({height: '*'})),
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
  address = import.meta.env.NG_APP_ADDRESS;
  address_complete: string = `http://${this.address}:5131/api/`;
  
  constructor(private dialog: MatDialog, private http: HttpClient) {}
  
  ngOnInit(): void {
    this.getDeviceUserRelaions();
    this.getUserRelations();
  }

  getDeviceUserRelaions(): void {
    
    const token = localStorage.getItem('token'); // Obtener el token del localStorage
  
    if (!token) {
      console.error('No se encontró el token de autenticación');
      return;
    }
  
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}` // Incluir el token en el encabezado
    });
  
    this.http.get<any[]>(`{address_complete}DeviceUserRelation/by-user`, { headers }).subscribe(
      (data) => {
        this.device_relation_user = data; // Asignamos los datos obtenidos a device_relation_user
        this.dataSourceDevice = this.device_relation_user; // Actualizamos dataSourceDevice
        this.table.renderRows(); // Forzar la actualización de la tabla
        console.log(this.device_relation_user);
      },
      (error) => {
        console.error('Error al obtener los tipos de dispositivos', error);
      }
    );
  }

  getDeviceUserRelaionsTest(): void {
    
    const token = localStorage.getItem('token'); // Obtener el token del localStorage
  
    if (!token) {
      console.error('No se encontró el token de autenticación');
      return;
    }
  
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}` // Incluir el token en el encabezado
    });
  
    this.http.get<any[]>(`{address_complete}DeviceUserRelation/by-user`, { headers }).subscribe(
      (data) => {
        this.device_relation_user = data; // Asignamos los datos obtenidos a device_relation_user
        this.dataSourceDevice = this.device_relation_user; // Actualizamos dataSourceDevice
        this.table.renderRows(); // Forzar la actualización de la tabla
        console.log(this.device_relation_user);
      },
      (error) => {
        console.error('Error al obtener los tipos de dispositivos', error);
      }
    );
  }

  getUserRelations(): void {
    
    const token = localStorage.getItem('token'); // Obtener el token del localStorage
  
    if (!token) {
      console.error('No se encontró el token de autenticación');
      return;
    }
  
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}` // Incluir el token en el encabezado
    });
  
    this.http.get<any[]>(`${this.address_complete}RelationUser/by-user`, { headers }).subscribe(
      (data) => {
        this.relation_user = data; // Asignamos los datos obtenidos a device_relation_user
        this.dataSource = this.relation_user; // Actualizamos dataSourceDevice
        this.table.renderRows(); // Forzar la actualización de la tabla
        console.log(this.relation_user);
      },
      (error) => {
        console.error('Error al obtener los tipos de dispositivos', error);
      }
    );
  }

  getUserRelationsTest(): void {
    
    const token = localStorage.getItem('token'); // Obtener el token del localStorage
  
    if (!token) {
      console.error('No se encontró el token de autenticación');
      return;
    }
  
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}` // Incluir el token en el encabezado
    });
  
    this.http.get<any[]>(`${this.address_complete}RelationUser/by-user`, { headers }).subscribe(
      (data) => {
        this.relation_user = data; // Asignamos los datos obtenidos a device_relation_user
        this.dataSource = this.relation_user; // Actualizamos dataSourceDevice
        this.table.renderRows(); // Forzar la actualización de la tabla
        console.log(this.relation_user);
      },
      (error) => {
        console.error('Error al obtener los tipos de dispositivos', error);
      }
    );
  }

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


  toggleRow(row: UserConection) {
    this.expandedElement = this.expandedElement === row ? null : row;
  }
  toggleRowDevice(row: UserDeviceRelation) {
    this.expandedElementDevice = this.expandedElementDevice === row ? null : row;
  }

  openModalDevice(): void {
    const dialogRef = this.dialog.open(ModalAddDeviceComponent, {
      width: '500px', // Ajusta el tamaño según sea necesario
      data: {} // Puedes pasar datos iniciales al modal aquí
    });

    dialogRef.afterClosed().subscribe(result => {
      this.getDeviceUserRelaionsTest();
      if (result) {
        console.log('Datos recibidos del formulario:', result);
      } else {
        console.log('El modal fue cerrado sin enviar datos.');
      }
    });
  }

  openModalConection(): void {
    const dialogRef = this.dialog.open(ModalAddConectionComponent, {
      width: '500px', // Ajusta el tamaño según sea necesario
      data: {} // Puedes pasar datos iniciales al modal aquí

      
    });

  
    dialogRef.afterClosed().subscribe(result => {
      this.getUserRelationsTest();

      if (result) {    
        console.log('Datos recibidos del formulario:', result);

      } else {
        console.log('El modal fue cerrado sin enviar datos.');
      }
    });
  }

  deleteDeviceRow(row: UserDeviceRelation) {
    const index = this.dataSourceDevice.indexOf(row);
    
    const token = localStorage.getItem('token'); // Obtener el token del localStorage
  
    if (!token) {
      console.error('No se encontró el token de autenticación');
      return;
    }
  
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}` // Incluir el token en el encabezado
    });

    var url_delete = `${this.address_complete}DeviceUserRelation/` + this.device_relation_user[this.dataSourceDevice.indexOf(row)]["id"]
  
    this.http.delete<any[]>(url_delete, { headers }).subscribe(
      (data) => {
  
      },
      (error) => {
        console.error('Error al obtener los tipos de dispositivos', error);
      }
    );

    if (index > -1) {
      this.dataSourceDevice.splice(index, 1);
      this.table.renderRows(); // Refresh the table to reflect the changes
    }

  }

  deleteConnectionRow(row: UserConection) {
    const index = this.dataSource.indexOf(row);

    const token = localStorage.getItem('token'); // Obtener el token del localStorage
  
    if (!token) {
      console.error('No se encontró el token de autenticación');
      return;
    }
  
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}` // Incluir el token en el encabezado
    });

    var url_delete = `${this.address_complete}/RelationUser/` + this.relation_user[this.dataSource.indexOf(row)]["id"]
  
    this.http.delete<any[]>(url_delete, { headers }).subscribe(
      (data) => {
  
      },
      (error) => {
        console.error('Error al obtener los tipos de dispositivos', error);
      }
    );

    if (index > -1) {
      this.dataSource.splice(index, 1);
      this.table.renderRows(); // Refresh the table to reflect the changes
    }
  }
}

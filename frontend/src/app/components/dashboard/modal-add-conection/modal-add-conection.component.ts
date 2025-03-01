import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog,
  MAT_DIALOG_DATA,
  MatDialogRef,
  MatDialogTitle,
  MatDialogContent,
  MatDialogActions,
  MatDialogClose, } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { CommonModule } from '@angular/common'; // Import CommonModule
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { Observable } from 'rxjs';
import { startWith, map } from 'rxjs/operators';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Component({
  selector: 'app-modal-add-conection',
  standalone: true,
  imports: [
    CommonModule, // Add CommonModule here
    MatFormFieldModule,
    MatInputModule,
    FormsModule,
    MatButtonModule,
    MatDialogTitle,
    MatDialogContent,
    MatDialogActions,
    MatDialogClose,
    ReactiveFormsModule,
    MatSelectModule,
    MatAutocompleteModule
  ],
  templateUrl: './modal-add-conection.component.html',
  styleUrl: './modal-add-conection.component.css'
})
export class ModalAddConectionComponent {
  formulario: FormGroup;

  filteredUsers: Observable<{ value: string, email: string }[]>;
  filteredDevices: Observable<{ id: string, nombre: string }[]>;
  filteredDevicesOwnUser: Observable<{ id: string, nombre: string }[]>;

  users: any[] = [];
  devices: any[] = [];
  devicesOwnUser: any[] = [];

  address = import.meta.env.NG_APP_ADDRESS;
  address_complete: string = `http://${this.address}:8080/api/`

  constructor(
    private http: HttpClient,
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<ModalAddConectionComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.formulario = this.fb.group({
      user: ['', Validators.required],
      device: ['', Validators.required],
      deviceOwner : ['', Validators.required],
      deviceId : ['', Validators.required],
      deviceOwnerId : ['', Validators.required]
    });

    this.filteredUsers = this.formulario.get('user')!.valueChanges.pipe(
      startWith(''),
      map(value => this._filterUsers(value || ''))
    );

    this.filteredDevices = this.formulario.get('device')!.valueChanges.pipe(
      startWith(''),
      map(id => this._filterDevices(id || ''))
    );

    this.filteredDevicesOwnUser = this.formulario.get('deviceOwner')!.valueChanges.pipe(
      startWith(''),
      map(id => this._filterDeviceOwner(id || ''))
    );
  }

  ngOnInit(): void {
    this.getUsers();
  }

  getUsers(): void {
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('No se encontró el token de autenticación');
      return;
    }

    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });

    this.http.get<any[]>(`${this.address_complete}user/all`, { headers }).subscribe(
      (data) => {
        this.users = data;
        console.log(this.users);
      },
      (error) => {
        console.error('Error al obtener los usuarios', error);
      }
    );
  }

  private _filterUsers(value: string): { value: string, email: string }[] {
    const filterValue = value.toLowerCase();
    return this.users.filter(option => option.email.toLowerCase().includes(filterValue));
  }

  private _filterDevices(value: string): { id: string, nombre: string }[] {
    const filterValue = value.toLowerCase();
    return this.devices.filter(option => option.nombre.toLowerCase().includes(filterValue));
  }

  private _filterDeviceOwner(value: string): { id: string, nombre: string }[] {
    const filterValue = value.toLowerCase();
    return this.devicesOwnUser.filter(option => option.nombre.toLowerCase().includes(filterValue));
  }
  

  onUserSelected(userEmail: string): void {

   
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('No se encontró el token de autenticación');
      return;
    }

    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });

    this.http.get<any[]>(`${this.address_complete}DeviceUserRelation/by-email?email=${userEmail}`, { headers }).subscribe(
      (data) => {
        this.devices = data;
        console.log(this.devices);
      },
      (error) => {
        console.error('Error al obtener los dispositivos', error);
      }
    );
  }

  ondeviceSelected(diveName: string): void {

    let selecteDevice = this.devices.find(devices => devices.nombre === diveName);

    if (selecteDevice) {
      this.formulario.patchValue({
        deviceId: selecteDevice.id, // Actualizar el ID del usuario
      });
    }

    const token = localStorage.getItem('token');
    if (!token) {
      console.error('No se encontró el token de autenticación');
      return;
    }

    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });

    this.http.get<any[]>(`${this.address_complete}DeviceUserRelation/by-user`, { headers }).subscribe(
      (data) => {
        this.devicesOwnUser = data;
        console.log(this.devicesOwnUser);
      },
      (error) => {
        console.error('Error al obtener los dispositivos', error);
      }
    );
  }
  
  onOwndeviceSelected(diveName: string): void {

    let deviceOwnId = this.devicesOwnUser.find(devices => devices.nombre === diveName);
    console.log(deviceOwnId);
    if (deviceOwnId) {
      this.formulario.patchValue({
        deviceOwnerId: deviceOwnId.id, // Actualizar el ID del usuario
      });
    }

  }

  enviarFormulario(): void {
   
    if (this.formulario.valid) {
       const token = localStorage.getItem('token');
        
            if (!token) {
              console.error('No se encontró el token de autenticación');
              return;
            }
        
            const headers = new HttpHeaders({
              'Authorization': `Bearer ${token}`
            });
        
            const formValue = {
              device: this.formulario.get('device')?.value,
              deviceId: this.formulario.get('deviceId')?.value,
              deviceOwner: this.formulario.get('deviceOwner')?.value,
              deviceOwnerId: this.formulario.get('deviceOwnerId')?.value,
              user: this.formulario.get('user')?.value
            };
        
            this.http.post(`${this.address_complete}RelationUser/create-relation`, formValue, { headers })
              .subscribe(
                (response) => {
                  console.log('Relación creada exitosamente', response);
                  this.dialogRef.close(response); // Cerrar el modal y devolver la respuesta
                },
                (error) => {
                  console.error('Error al crear la relación', error);
                  // Mostrar mensaje de error en la UI
                }
              );
      this.dialogRef.close(this.formulario.value);
    }
  }
}
import { Component, Inject, OnInit } from '@angular/core';
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
import { CommonModule } from '@angular/common';
import { HttpClient, HttpHeaders } from '@angular/common/http'; // Import HttpClient
import { takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { MatSnackBar } from '@angular/material/snack-bar';
@Component({
  selector: 'app-modal-add-device',
  standalone: true,
  imports: [
    CommonModule,
    MatFormFieldModule,
    MatInputModule,
    FormsModule,
    MatButtonModule,
    MatDialogTitle,
    MatDialogContent,
    MatDialogActions,
    MatDialogClose,
    ReactiveFormsModule,
    MatSelectModule
  ],
  templateUrl: './modal-add-device.component.html',
  styleUrl: './modal-add-device.component.css'
})
export class ModalAddDeviceComponent implements OnInit {
  formulario: FormGroup;
  deviceTypes: any[] = []; // Inicializamos deviceTypes como un array vacío
  private destroy$ = new Subject<void>();
  address = import.meta.env.NG_APP_ADDRESS;
  port_user = import.meta.env.NG_APP_PORT_USER;
  address_complete: string = `http://${this.address}:${this.port_user}/userservice/v1/`;

  constructor(
    private fb: FormBuilder,
    private snackBar: MatSnackBar,
    public dialogRef: MatDialogRef<ModalAddDeviceComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private http: HttpClient // Inyectamos HttpClient
  ) {
    // Inicializamos el formulario
    this.formulario = this.fb.group({
      deviceType: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.obtenerTiposDeDispositivos();
  }

  obtenerTiposDeDispositivos(): void {
    const token = localStorage.getItem('token'); // Obtener el token del localStorage
  
    if (!token) {
      console.error('No se encontró el token de autenticación');
      return;
    }
  
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}` // Incluir el token en el encabezado
    });

    this.http.get<any[]>(`${this.address_complete}device`, {headers}).subscribe(
      (data) => {
        this.deviceTypes = data; // Asignamos los datos obtenidos a deviceTypes
      },
      (error) => {
        console.error('Error al obtener los tipos de dispositivos', error);
      }
    );
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
  
      const deviceUserRelation = {
        deviceName: this.formulario.value.deviceType
      };
  
      this.http.post(`${this.address_complete}DeviceUserRelation`, deviceUserRelation, { headers })
        .pipe(takeUntil(this.destroy$))
        .subscribe(
          (response) => {
            this.snackBar.open("Se agregó el dispositivo correctamente.", 'Cerrar', { duration: 5000 })
            this.dialogRef.close(response); // Cerrar el modal y devolver la respuesta
          },
          (error) => {
            this.snackBar.open("Error al agregar el dispositivo.", 'Cerrar', { duration: 5000 });
            // Mostrar mensaje de error en la UI
          }
        );
    }
  }
}
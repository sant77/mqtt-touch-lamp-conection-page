import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogActions, MatDialogClose, MatDialogContent, MatDialogRef, MatDialogTitle } from '@angular/material/dialog';
import { ModalAddConectionComponent } from '../../dashboard/modal-add-conection/modal-add-conection.component';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatAutocompleteModule } from '@angular/material/autocomplete';

@Component({
  selector: 'app-forgot-password-dialog',
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
  templateUrl: './forgot-password-dialog.component.html',
  styleUrl: './forgot-password-dialog.component.css'
})
export class ForgotPasswordDialogComponent {
  recoverForm: FormGroup;
  email: string = '';
  address = import.meta.env.NG_APP_ADDRESS;
  port_user = import.meta.env.NG_APP_PORT_USER;
  address_complete: string = `http://${this.address}:${this.port_user}/userservice/v1/`;
  constructor(
    private http: HttpClient,
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<ModalAddConectionComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.recoverForm = this.fb.group({
      email: ['', Validators.required],
    });
  }

 
 
  sendRecoveryEmail(): void {
     
      if (this.recoverForm.valid) {
         const token = localStorage.getItem('token');
          
              if (!token) {
                console.error('No se encontró el token de autenticación');
                return;
              }
          
              const headers = new HttpHeaders({
                'Authorization': `Bearer ${token}`
              });
          
              const formValue = {
                email: this.recoverForm.get('email')?.value,
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
        this.dialogRef.close(this.recoverForm.value);
      }
    }
}

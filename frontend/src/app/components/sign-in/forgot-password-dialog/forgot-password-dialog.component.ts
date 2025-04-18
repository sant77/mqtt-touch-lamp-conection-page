import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogActions, MatDialogClose, MatDialogContent, MatDialogRef, MatDialogTitle } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-forgot-password-dialog',
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
    ReactiveFormsModule
  ],
  templateUrl: './forgot-password-dialog.component.html',
  styleUrl: './forgot-password-dialog.component.css'
})
export class ForgotPasswordDialogComponent {
  recoverForm: FormGroup;
  isLoading = false;
  address = import.meta.env.NG_APP_ADDRESS;
  port_user = import.meta.env.NG_APP_PORT_USER;
  address_complete: string = `http://${this.address}:${this.port_user}/userservice/v1/user/`;

  constructor(
    private http: HttpClient,
    private fb: FormBuilder,
    private snackBar: MatSnackBar,
    public dialogRef: MatDialogRef<ForgotPasswordDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.recoverForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
    });
  }

  sendRecoveryEmail(): void {
    if (this.recoverForm.valid) {
      this.isLoading = true;
      
      const formValue = {
        email: this.recoverForm.get('email')?.value,
      };

      this.http.post(`${this.address_complete}request-password-reset`, formValue)
        .subscribe({
          next: () => {
            this.snackBar.open('Si el email existe, se ha enviado un PIN de recuperación.', 'Cerrar', {
              duration: 5000,
            });
            this.dialogRef.close();
          },
          error: (error) => {
            console.error('Error al solicitar recuperación', error);
            this.snackBar.open('Error al procesar la solicitud', 'Cerrar', {
              duration: 5000,
            });
          },
          complete: () => {
            this.isLoading = false;
          }
        });
    }
  }
}
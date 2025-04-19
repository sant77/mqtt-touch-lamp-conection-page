import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule
  ],
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css']
})
export class ResetPasswordComponent {
  resetForm: FormGroup;
  
  email: string | null = null;
  address = import.meta.env.NG_APP_ADDRESS;
  port_user = import.meta.env.NG_APP_PORT_USER;
  address_complete: string = `http://${this.address}:${this.port_user}/userservice/v1/user/`;

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private snackBar: MatSnackBar,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.resetForm = this.fb.group({
      token: ['', [Validators.required, Validators.pattern(/^\d{6}$/)]],
      newPassword: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', Validators.required]
    }, { validator: this.passwordMatchValidator });

    this.route.queryParams.subscribe(params => {
      this.email = params['email'] || null;
      if (!this.email) {
        this.snackBar.open('Correo electrónico no proporcionado', 'Cerrar', { duration: 5000 });
        this.router.navigate(['/login']);
      }
    });
  }

  passwordMatchValidator(formGroup: FormGroup) {
    const password = formGroup.get('newPassword')?.value;
    const confirmPassword = formGroup.get('confirmPassword')?.value;
    return password === confirmPassword ? null : { mismatch: true };
  }

  resetPassword(): void {
    
    if (this.resetForm.valid && this.email) {
    
      const formValue = {
        email: this.email,
        token: this.resetForm.get('token')?.value,
        newPassword: this.resetForm.get('newPassword')?.value
      };

      this.http.post(`${this.address_complete}reset-password`, formValue)
        .subscribe({
          next: () => {
            this.snackBar.open('Contraseña actualizada correctamente', 'Cerrar', { duration: 5000 });
            this.router.navigate(['/login']);
          },
          error: (error) => {
    
            this.snackBar.open('PIN inválido o expirado', 'Cerrar', { duration: 5000 });
          },
          complete: () => {
            
          }
        });
    }
  }
}
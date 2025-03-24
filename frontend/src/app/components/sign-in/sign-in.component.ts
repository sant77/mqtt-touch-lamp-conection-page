import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { Router, RouterLink } from '@angular/router';
import { LoginUser } from './login-service';
import { ToastrService } from 'ngx-toastr';
import { NavbarComponent } from '../home/navbar/navbar.component';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-sign-in',
  standalone: true,
  imports: [CommonModule, 
            MatInputModule, 
            MatButtonModule, 
            ReactiveFormsModule,
            NavbarComponent,
            MatToolbarModule, 
            MatIconModule, 
            RouterLink
          ],
  templateUrl: './sign-in.component.html',
  styleUrl: './sign-in.component.css'
})
export class SignInComponent {
  registerForm: FormGroup;
    errorMessage: string | null = null; // Para manejar mensajes de error
  
    constructor(
      private fb: FormBuilder,
      private myService: LoginUser,
      private router: Router, // Inyectar el Router
      private toastr: ToastrService
    
    ) {
      this.registerForm = this.fb.group({
        email: ['', [Validators.required, Validators.email]],
        password: ['', Validators.required]
      });
    }
  
    onSubmit(): void {
      if (this.registerForm.valid) {
        const formData = this.registerForm.value;
        this.myService.login(formData).subscribe({
          next: (response) => {
            localStorage.setItem('token', response.token);
            console.log('Registro exitoso', response);
            this.router.navigate(['/dashboard']); // Redirigir al dashboard
          },
          error: (err) => {
           // Manejo de errores según el código de estado HTTP
        if (err.status === 400 || err.status === 401) {
          const errorMessage = err.error?.error || 'Usuario o contraseña incorrecta'; // Si el backend envía un JSON con un campo `error`
          this.toastr.error(errorMessage, '¡Error!');
        } else if (err.status === 500) {
          this.toastr.error('Ocurrió un error interno en el servidor. Inténtalo más tarde.', '¡Error!');
        } else {
          
          this.toastr.error('Error desconocido. Inténtalo más tarde.', '¡Error!');
        }
          }
        });
      } else {
       
        this.errorMessage = 'Por favor completa todos los campos correctamente.'; // Validación en el cliente
      }
    }
}

import { Component } from '@angular/core';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { RegisterUser } from './register-service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ToastrService } from 'ngx-toastr';
import { NavbarComponent } from '../home/navbar/navbar.component';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-sign-up',
  standalone: true,
  imports: [CommonModule, 
            MatInputModule, 
            MatButtonModule, 
            ReactiveFormsModule, 
            NavbarComponent, 
            MatToolbarModule, 
            MatIconModule, 
            RouterLink],
  templateUrl: './sign-up.component.html',
  styleUrl: './sign-up.component.css'
})
export class SignUpComponent {
  registerForm: FormGroup;
  errorMessage: string | null = null; // Para manejar mensajes de error

  constructor(
    private fb: FormBuilder,
    private myService: RegisterUser,
    private router: Router, // Inyectar el Router
    private toastr: ToastrService
  ) {
    this.registerForm = this.fb.group({
      name: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
      password2: ['', Validators.required]
    });
  }

  onSubmit(): void {
    if (this.registerForm.valid) {
      const formData = this.registerForm.value;

      // Comparar las contraseñas
      if (formData.password !== formData.password2) {
        this.errorMessage = 'Las contraseñas no coinciden.';
        return;
      }

      // Eliminar password2 del objeto formData
      delete formData.password2;

      // Enviar los datos al servicio
      this.myService.register(formData).subscribe({
        next: (response) => {
          console.log('Registro exitoso', response);
          this.toastr.success("Usuario creado", "¡Exito!")
          this.router.navigate(['/dashboard']); // Redirigir al dashboard
        },
        error: (err) => {
          this.toastr.error("Usuario ya existe", "¡Error!")
          console.error('Error en el registro', err);
        }
      });
    } else {
      this.errorMessage = 'Por favor completa todos los campos correctamente.'; // Validación en el cliente
    }
  }
}
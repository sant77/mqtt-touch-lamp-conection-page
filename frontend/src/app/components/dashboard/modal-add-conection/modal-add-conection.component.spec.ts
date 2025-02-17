import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ModalAddConectionComponent } from './modal-add-conection.component';

describe('ModalAddConectionComponent', () => {
  let component: ModalAddConectionComponent;
  let fixture: ComponentFixture<ModalAddConectionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ModalAddConectionComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ModalAddConectionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

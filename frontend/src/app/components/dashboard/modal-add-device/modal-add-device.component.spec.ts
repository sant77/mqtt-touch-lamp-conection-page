import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ModalAddDeviceComponent } from './modal-add-device.component';

describe('ModalAddDeviceComponent', () => {
  let component: ModalAddDeviceComponent;
  let fixture: ComponentFixture<ModalAddDeviceComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ModalAddDeviceComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ModalAddDeviceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

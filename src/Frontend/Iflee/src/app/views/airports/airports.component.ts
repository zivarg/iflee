import { Component, OnInit } from '@angular/core';
import {AircraftsService} from '../../services/aircrafts.service';

@Component({
  templateUrl: 'airports.component.html',
  providers: [AircraftsService],
})
export class AirportsComponent implements OnInit {
  ngOnInit(): void {
  }
}

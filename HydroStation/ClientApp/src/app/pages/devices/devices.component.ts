import { Component, OnInit, Pipe, PipeTransform } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { DatePipe } from '@angular/common';
import { NgbModal, ModalDismissReasons } from '@ng-bootstrap/ng-bootstrap';
import Chart from 'chart.js';

// core components
import {
  chartOptions,
  parseOptions,
} from "../../variables/charts";

// Colors
var colors = {
  gray: {
    100: '#f6f9fc',
    200: '#e9ecef',
    300: '#dee2e6',
    400: '#ced4da',
    500: '#adb5bd',
    600: '#8898aa',
    700: '#525f7f',
    800: '#32325d',
    900: '#212529'
  },
  theme: {
    'default': '#172b4d',
    'primary': '#5e72e4',
    'secondary': '#f4f5f7',
    'info': '#11cdef',
    'success': '#2dce89',
    'danger': '#f5365c',
    'warning': '#fb6340'
  },
  black: '#12263F',
  white: '#FFFFFF',
  transparent: 'transparent',
};

export interface DeviceConfig {
  friendlyName: string;
  dosingConfig: number;
}

export const phChartConfig = {
  options: {
    responsive: true,
    tooltips: {
      mode: 'index',
      intersect: false,
    },
    hover: {
      mode: 'nearest',
      intersect: true
    },
    scales: {
      yAxes: [{
        display: true,
        scaleLabel: {
          display: true,
          labelString: 'PH Value'
        },
        ticks: {
          max: 7,
          min: 5,
          stepSize: .5,
          callback: function (value) {
            return value;
          }
        }
      }]
    }
  },
  data: {
    labels: [],
    datasets: [{
      label: 'pH',
      fill: false,
      pointRadius: 10,
      backgroundColor: colors.theme.success,
      borderColor: colors.theme.success,
      data: []
    }]
  }
}

export const temperatureChartConfig = {
  options: {
    responsive: true,
    tooltips: {
      mode: 'index',
      intersect: false,
    },
    hover: {
      mode: 'nearest',
      intersect: true
    },
    scales: {
      yAxes: [{
        display: true,
        gridLines: {
          display: false
        },
        scaleLabel: {
          display: true,
          labelString: 'Temperature Celcius'
        },
        ticks: {
          max: 25,
          min: 18,
          stepSize: 1,
          callback: function (value) {
            return value;
          }
        }
      }]
    }
  },
  data: {
    labels: [],
    datasets: [{
      label: 'pH',
      fill: false,
      pointRadius: 10,
      backgroundColor: colors.theme.success,
      borderColor: colors.theme.success,
      data: []
    }]
  }
}

@Component({
  selector: 'devices',
  templateUrl: './devices.component.html',
  styleUrls: ['./devices.component.scss'],
  providers: [DatePipe]
})
export class DevicesComponent implements OnInit {
  private headers: HttpHeaders;
  private devicesURL: string = '/device/';
  private hourlyHistoricalURL: string = '/device/GetHourlyHistorical';

  public PHValue: string;
  public WaterTemperature: string;
  public Temperature: string;
  public Humidity: string;

  public deviceConfigFriendlyName: string;
  public deviceConfigDosingConfig: number = 0;

  public deviceId: string;
  public lastUpdated: Date;
  public lastUpdatedText: string;

  public datasets: any;
  public data: any;
  public phChart;
  public waterTemperatureChart;
  public temperatureChart;
  public humidityChart;

  constructor(private route: ActivatedRoute, private http: HttpClient, public datepipe: DatePipe, private modalService: NgbModal) {
    this.headers = new HttpHeaders({ 'Content-Type': 'application/json; charset=utf-8' });
  }

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      this.deviceId = params.get('deviceId');

      this.getDeviceInfo(true);

      setInterval(() => {
        this.getDeviceInfo(false);
      }, 1000);
    });

    parseOptions(Chart, chartOptions());

    var phChartElement = document.getElementById('ph-data');
    var waterTemperatureChartElement = document.getElementById('water-temperature-data');
    var temperatureChartElement = document.getElementById('temperature-data');
    var humidityChartElement = document.getElementById('humidity-data');

    this.phChart = new Chart(phChartElement, {
      type: 'line',
      options: phChartConfig.options,
      data: phChartConfig.data
    });

    this.waterTemperatureChart = new Chart(waterTemperatureChartElement, {
      type: 'line',
      options: temperatureChartConfig.options,
      data: temperatureChartConfig.data
    });

    this.temperatureChart = new Chart(temperatureChartElement, {
      type: 'line',
      options: temperatureChartConfig.options,
      data: temperatureChartConfig.data
    });

    this.humidityChart = new Chart(humidityChartElement, {
      type: 'line',
      options: temperatureChartConfig.options,
      data: temperatureChartConfig.data
    });

    this.loadHourlyCharts();
  }

  public open(content) {
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title', windowClass: "fade" }).result.then((result) => {

    }, (reason) => {

    });
  }


  public onSubmit(modal) {
    this.http.post(this.devicesURL + this.deviceId, { friendlyName: this.deviceConfigFriendlyName, dosingConfig: +this.deviceConfigDosingConfig }, { headers: this.headers }).subscribe((data: any[]) => {
      this.modalService.dismissAll();
    });
  }

  public getDeviceName() {
    return this.deviceConfigFriendlyName != null ? this.deviceConfigFriendlyName : "Device - " + this.deviceId;
  }

  public getDosingConfigName() {
    switch (this.deviceConfigDosingConfig) {
      case 0:
        return "Off";
      case 1:
        return "Down";
      case 2:
        return "Up";
    }
  }

  public onDosingConfigChange(value) {
    this.deviceConfigDosingConfig = +value;
  }

  public onFriendlyNameChange(value) {
    this.deviceConfigFriendlyName = value;

  }
  public loadHourlyCharts() {
    this.http.get(this.hourlyHistoricalURL, { headers: this.headers }).subscribe((data: any[]) => {
      console.log(data);
      var keys = Object.keys(data);
      var values = Object.values(data);

      var dateKeys = values.map((value, index, array) => {
        var date = new Date(keys[index]);
        return this.datepipe.transform(date, "hh:mm");
      });

      this.setPhChart(values, dateKeys);
      this.setWaterTemperatureChart(values, dateKeys);
      this.setTemperatureChart(values, dateKeys);
      this.setHumidityChart(values, dateKeys);
    });
  }


  private setPhChart(values: any[], dateKeys: string[]) {
    var phValues = values.map((value, index, array) => {
      return value ? value.phValue : null;
    });

    var max = Math.max.apply(Math, phValues.map(function (o) { return o ? o : -1; }));
    var min = Math.min.apply(Math, phValues.map(function (o) { return o ? o : 100; }));

    this.phChart.data.labels = dateKeys;
    this.phChart.options.scales.yAxes[0].ticks.max = Math.ceil(max + .25);
    this.phChart.options.scales.yAxes[0].ticks.min = Math.floor(min - .25);
    this.phChart.data.datasets[0].data = phValues;

    this.phChart.update();
  }

  private setWaterTemperatureChart(values: any[], dateKeys: string[]) {
    var waterTempValues = values.map((value, index, array) => {
      return value ? value.waterTemperature : null;
    });

    var max = Math.max.apply(Math, waterTempValues.map(function (o) { return o ? o : -1; }));
    var min = Math.min.apply(Math, waterTempValues.map(function (o) { return o ? o : 100; }));

    this.waterTemperatureChart.data.labels = dateKeys;
    this.waterTemperatureChart.options.scales.yAxes[0].ticks.max = Math.ceil(max + .25);
    this.waterTemperatureChart.options.scales.yAxes[0].ticks.min = Math.floor(min - .25);
    this.waterTemperatureChart.data.datasets[0].data = waterTempValues;

    this.waterTemperatureChart.update();
  }

  private setTemperatureChart(values: any[], dateKeys: string[]) {
    var tempValue = values.map((value, index, array) => {
      return value ? value.temperature : null;
    });

    var max = Math.max.apply(Math, tempValue.map(function (o) { return o ? o : -1; }));
    var min = Math.min.apply(Math, tempValue.map(function (o) { return o ? o : 100; }));

    this.temperatureChart.data.labels = dateKeys;
    this.temperatureChart.options.scales.yAxes[0].ticks.max = Math.ceil(max + .25);
    this.temperatureChart.options.scales.yAxes[0].ticks.min = Math.floor(min - .25);
    this.temperatureChart.data.datasets[0].data = tempValue;

    this.temperatureChart.update();
  }

  private setHumidityChart(values: any[], dateKeys: string[]) {
    var humidityValue = values.map((value, index, array) => {
      return value ? value.humidity : null;
    });

    var max = Math.max.apply(Math, humidityValue.map(function (o) { return o ? o : -1; }));
    var min = Math.min.apply(Math, humidityValue.map(function (o) { return o ? o : 100; }));

    this.humidityChart.data.labels = dateKeys;
    this.humidityChart.options.scales.yAxes[0].ticks.max = Math.ceil(max + .25);
    this.humidityChart.options.scales.yAxes[0].ticks.min = Math.floor(min - .25);
    this.humidityChart.data.datasets[0].data = humidityValue;

    this.humidityChart.update();
  }

  private getDeviceInfo(loadConfig: boolean) {
    this.http.get(this.devicesURL + this.deviceId, { headers: this.headers }).subscribe((data: any) => {
      this.PHValue = data.deviceData.phValue;
      this.WaterTemperature = data.deviceData.waterTemperature;
      this.Temperature = data.deviceData.temperature;
      this.Humidity = data.deviceData.humidity;

      if (loadConfig) {
        this.deviceConfigFriendlyName = data.deviceConfig.friendlyName;
        this.deviceConfigDosingConfig = data.deviceConfig.dosingConfig;
      }

      this.lastUpdated = new Date();
      this.lastUpdatedText = this.datepipe.transform(this.lastUpdated, 'medium');
    });
  }
}

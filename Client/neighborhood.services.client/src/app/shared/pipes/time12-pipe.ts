import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'time12',
})
export class Time12Pipe implements PipeTransform {
  transform(time: string): string {
    if (!time)
      return '';

    return new Date(`1970-01-01T${time}`)
      .toLocaleTimeString('en-US', {
        hour: 'numeric',
        minute: '2-digit',
        hour12: true
      })
  }
}

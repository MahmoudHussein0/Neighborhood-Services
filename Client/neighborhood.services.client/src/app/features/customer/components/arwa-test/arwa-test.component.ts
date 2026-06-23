import { ChangeDetectionStrategy,Component ,signal,inject} from '@angular/core';
import { HttpClient } from '@angular/common/http';





@Component({
  selector: 'app-arwa-test',
  imports: [],
  templateUrl: './arwa-test.component.html',
  styleUrl: './arwa-test.component.css',
})
export class ArwaTestComponent {


   selectedFile = signal<File | null>(null);
   private http = inject(HttpClient);

   // Replace with your Cloudinary cloud name and upload preset
  cloudName = 'duiqtca36';
  uploadPreset = 'letTry';
  cloudinaryURL = `https://api.cloudinary.com/v1_1/${this.cloudName}/image/upload`;
  myApiKey='389123478731382'
  myApiSecret='E06EEtHJP0ebWvPny_8ZU2dp0UI'
  timestamp:any;

 onFileSelected(event: Event) {
   const input = event.target as HTMLInputElement;
   if (input.files && input.files.length > 0) {
     this.selectedFile.set(input.files[0]);
   }
 }

 onUpload() {
   const file = this.selectedFile();
    this.timestamp = Math.floor(Date.now() / 1000);
   // const signatureString = `timestamp=${this.timestamp}&upload_preset=${this.uploadPreset}${this.myApiSecret}`;
   const signatureString = `timestamp=${this.timestamp}${this.myApiSecret}`;


   if (!file) return;

   const formData = new FormData();
   formData.append('file', file);
    //formData.append('upload_preset', this.uploadPreset);
      formData.append('api_key', this.myApiKey);
    formData.append('timestamp', this.timestamp);
     console.log("Timestamp is:")
    console.log(this.timestamp)
   formData.append('signature', signatureString);
    console.log("Signature is:")
    console.log(signatureString)


   // Example request to send the image to a server. We'll change this later
   this.http.post(`https://api.cloudinary.com/v1_1/${this.cloudName}/image/upload`, formData)
     .subscribe({
      next: (response) => {
        console.log('Upload success!', response);
      },
      error: (err) => {
        console.error('Upload failed:', err.error); // ✅ log exact error message
      }
    });
 }

}

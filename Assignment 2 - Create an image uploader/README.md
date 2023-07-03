# Image Uploader  

This assignemnet include implementing an image uploader form using ASP.NET Core Minimal API.

## Main tasks

Create a form with the following input
- Title of image (required).
- A file input that accept only "jpeg, png or gif" (required).

When submission is successful, redirect to a page with unique id. URL form: "/picture/{xxxx}" where xxxx is uploaded image id.

## Screenshots

### Input Form
![form](https://github.com/MillaniaSameh/imageUploader-assignment/assets/76593662/78bf17c7-3833-40a6-96c9-b8dc875d91b7)

### Successful submission 
![success](https://github.com/MillaniaSameh/imageUploader-assignment/assets/76593662/0bdbfe20-e4bf-496e-b322-e1341f4776d9)

### Validations 
The inputs must be enetered before submitting the form.
![Validation1](https://github.com/MillaniaSameh/imageUploader-assignment/assets/76593662/93644c7c-745b-4eaf-94b5-6273d128c682)
![Validation2](https://github.com/MillaniaSameh/imageUploader-assignment/assets/76593662/8b10cb91-530c-4246-abdb-5b541448acfe)

The File input can only accept jpeg, png or gif.
![Validation3](https://github.com/MillaniaSameh/imageUploader-assignment/assets/76593662/ebfa79e5-6a63-43ca-a37f-a24cc18cd153)

The user may change that extension. A 404 page is diplayed in that case.
![Validation4](https://github.com/MillaniaSameh/imageUploader-assignment/assets/76593662/550a120b-ca8f-4d76-939d-f8e3b69857b4)
![Validation5](https://github.com/MillaniaSameh/imageUploader-assignment/assets/76593662/8e480d2a-1c6c-4f5e-b0c0-c5874e071632)

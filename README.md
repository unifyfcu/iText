# UNIFY Financial Credit Union's iText Implementation <hr />
This project is publically available under the license listed.  This project utilizes [iText7](https://itextpdf.com/) solution to provide the tools to the organization with PDF management.

## Use <hr />
The application uses Azure's App Registration flow.  You will need to setup your own App Registration and associated security controls.  In addition, the system uses DataDog for it's logging.  If you wish to use the same you will need to register with DataDog.  You can also switch to any other type of Serilog Sink for your logging purposes.  Finally, the system uses Azure Key Vaults for any sensitive information as well as a pipeline token replacement.  These will need to be addressed for your own use.

## Endpoints <hr />

### Get Fields
Retrieves the list of Form Fields in the provided PDF
Request: *url*/fields
Body: typeof(HandlerDto)
<br />
Response: string[]

### Get Single
Returns a Single PDF Document
Request: *url*/
Body: typeof(HandlerDto)
<br />
Response: "application/pdf"

### Get
Attempts to generate a bulk collection of PDF documents to download
Request: *url*/archive
Body: typeof(HandlerDto)
<br />
Response: "application/octet-stream"
## Models <hr />

```typescript
export class HandlerDto {
	/**
	 * Template URI; must be a PDF location
	 */
	public templateUrl: string|null;
	/**
	 * Template Byte Array stored as a base64 string
	 */
	public templateData: string|null;

	/**
	 * Key / Value pair of ACRO Form Field Names and the associated values tied to it
	 */
	public properties: Map<string, string>[] = [];
}
```

export interface AboutContent {
  text: string | null;
  photoPath: string | null;
}

export interface ContactsContent {
  address: string | null;
  phone: string | null;
  hours: string | null;
  mapEmbed: string | null;
}

export interface AdminSiteContentResponse {
  about: AboutContent;
  contacts: ContactsContent;
}

export interface AboutFormData {
  text: string;
  photoPath: string;
}

export interface ContactsFormData {
  address: string;
  phone: string;
  hours: string;
  mapEmbed: string;
}

export interface UploadImageResponse {
  relativePath: string;
  url: string;
  fileName: string;
  size: number;
}

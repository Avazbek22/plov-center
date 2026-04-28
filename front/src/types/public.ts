export interface PublicMenuDish {
  id: string;
  name: string;
  description: string | null;
  price: number;
  photos: string[];
  sortOrder: number;
}

export interface PublicMenuCategory {
  id: string;
  name: string;
  sortOrder: number;
  dishes: PublicMenuDish[];
}

export interface PublicMenu {
  categories: PublicMenuCategory[];
}

export interface PublicAbout {
  text: string | null;
  photoPath: string | null;
}

export interface PublicContacts {
  address: string | null;
  phone: string | null;
  hours: string | null;
  mapEmbed: string | null;
}

export interface PublicSiteContent {
  about: PublicAbout;
  contacts: PublicContacts;
}

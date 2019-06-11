export interface Blog {
  id: number;

  title: string;
  slug: string;
  description: string;
  body: string;
  tags: string[];
  publishedDate: Date;
  isPublished: boolean;
}

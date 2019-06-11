export interface BlogDto {
  title: string;
  description: string;
  body: string;
  tags: string[];
  publishedDate: Date;
  isPublished: boolean;
}

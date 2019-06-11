/* Defines the user entity */
export class User {
  id_token: string;
  access_token: string;
  user_Name: string;
  start_at: Date;
  expires_at: Date;

  isExpired() {
    return new Date(this.expires_at) < new Date();
  }
}

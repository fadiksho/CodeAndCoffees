import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { LoginVM } from './login-vm';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  apiRoot = environment.apiRoot;

  constructor(private http: HttpClient) {}

  isLoggedIn(): boolean {
    return false;
  }

  login(vm: LoginVM): Observable<any> {
    return this.http.post(this.apiRoot + 'login', vm);
  }
  logout() {}
}

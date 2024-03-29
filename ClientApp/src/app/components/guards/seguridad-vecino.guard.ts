import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { Router } from '@angular/router';
import { VecinoService } from '../../services/vecino.service';

@Injectable({
  providedIn: 'root'
})
export class SeguridadVecinoGuard implements CanActivate {
  constructor(private router: Router, private vecinoService: VecinoService) {

  }
  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
      
      if (!this.vecinoService.isLoggedIn()) {
        return this.router.navigate(["/error-pagina-login"]).then(() => false);
      }

      return this.vecinoService.ObtenerVariableSession();
    }
  
}


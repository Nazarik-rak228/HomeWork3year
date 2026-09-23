package org.example.pw3models_no_db.service;

import org.example.pw3models_no_db.model.RoleModel;
import org.example.pw3models_no_db.model.UserModel;
import org.springframework.security.core.GrantedAuthority;
import org.springframework.security.core.authority.SimpleGrantedAuthority;
import org.springframework.security.core.userdetails.UserDetails;

import java.util.Collection;
import java.util.List;

public class UserPrincipal implements UserDetails {

    private final UserModel user;

    public UserPrincipal(UserModel user) {
        this.user = user;
    }

    @Override
    public Collection<? extends GrantedAuthority> getAuthorities() {
        RoleModel role = user.getRole();
        if (role == null || role.getRolName() == null) {
            System.out.println("⚠️ Роль отсутствует!");
            return List.of();
        }

        String roleName = role.getRolName().trim().toUpperCase();
        if (!roleName.startsWith("ROLE_")) {
            roleName = "ROLE_" + roleName;
        }

        System.out.println("🔑 Выдана роль: " + roleName);   // ← для отладки
        return List.of(new SimpleGrantedAuthority(roleName));
    }

    @Override
    public String getPassword() {
        return user.getPassword();
    }

    @Override
    public String getUsername() {
        return user.getUsername();
    }

    @Override
    public boolean isAccountNonExpired() { return true; }
    @Override
    public boolean isAccountNonLocked() { return true; }
    @Override
    public boolean isCredentialsNonExpired() { return true; }
    @Override
    public boolean isEnabled() { return true; }
}
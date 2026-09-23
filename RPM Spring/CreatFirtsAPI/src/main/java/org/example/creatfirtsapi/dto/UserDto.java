package org.example.creatfirtsapi.dto;

import jakarta.persistence.*;
import jakarta.validation.constraints.NotBlank;



@Entity
@Table(name="users")
public class UserDto {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private int id;
    @NotBlank(message = "Нельзя оставить поле пустым")
    private String username;
    @NotBlank(message = "Нельзя оставить поле пустым")
    private String email;
    @NotBlank(message = "Нельзя оставить поле пустым")
    private String password;
    @ManyToOne(fetch = FetchType.EAGER)
    @JoinColumn(name = "role_id", nullable = false)
    private RoleDto role;

    public UserDto() {

    }
    public UserDto(String username, String email, String password, RoleDto role) {

        this.username = username;
        this.email = email;
        this.password = password;
        this.role = role;
    }

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public String getUsername() {
        return username;
    }

    public void setUsername(String username) {
        this.username = username;
    }

    public String getEmail() {
        return email;
    }

    public void setEmail(String email) {
        this.email = email;
    }

    public String getPassword() {
        return password;
    }

    public void setPassword(String password) {
        this.password = password;
    }

    public RoleDto getRole() {
        return role;
    }

    public void setRole(RoleDto role) {
        this.role = role;
    }
}

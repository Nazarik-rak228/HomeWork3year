package org.example.creatfirtsapi.controller;

import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.tags.Tag;
import jakarta.validation.Valid;
import org.example.creatfirtsapi.dto.RoleDto;
import org.example.creatfirtsapi.repository.RoleRepository;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/role")
@Tag(name = "Roles", description = "CRUD для ролей")
public class RoleController {

    private final RoleRepository roleRepository;

    public RoleController(RoleRepository roleRepository) {
        this.roleRepository = roleRepository;
    }

    // GET ALL
    @Operation(summary = "Получить все роли", description = "Возвращает список всех ролей")
    @GetMapping
    public List<RoleDto> getAll() {
        return roleRepository.findAll();
    }

    // GET BY ID
    @Operation(summary = "Получить роль по id", description = "Возвращает роль по её id")
    @GetMapping("/{id}")
    public ResponseEntity getById(@PathVariable int id) {
        return roleRepository.findById(id)
                .<ResponseEntity>map(ResponseEntity::ok)
                .orElseGet(() -> ResponseEntity.status(HttpStatus.NOT_FOUND).body("Role not found"));
    }

    // CREATE
    @Operation(summary = "Создать роль", description = "Создаёт новую роль")
    @PostMapping
    public ResponseEntity addRole(@RequestParam String rolName) {

        RoleDto role = new RoleDto(0, rolName);
        roleRepository.save(role);

        return ResponseEntity.status(HttpStatus.CREATED).body(role);
    }

    // UPDATE
    @Operation(summary = "Обновить роль", description = "Обновляет роль по id")
    @PutMapping
    public ResponseEntity updateRole(@RequestParam int id,
                                     @RequestParam String rolName) {

        if (!roleRepository.existsById(id)) {
            return ResponseEntity.status(HttpStatus.NOT_FOUND)
                    .body("Role not found");
        }

        RoleDto role = new RoleDto(id, rolName);
        roleRepository.save(role);

        return ResponseEntity.ok(role);
    }

    // DELETE
    @Operation(summary = "Удалить роль", description = "Удаляет роль по id")
    @DeleteMapping("/{id}")
    public ResponseEntity deleteRole(@PathVariable int id) {

        if (!roleRepository.existsById(id)) {
            return ResponseEntity.status(HttpStatus.NOT_FOUND)
                    .body("Role not found");
        }

        roleRepository.deleteById(id);
        return ResponseEntity.ok("Deleted");
    }
}
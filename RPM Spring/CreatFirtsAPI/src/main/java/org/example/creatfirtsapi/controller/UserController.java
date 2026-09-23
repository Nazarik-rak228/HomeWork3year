package org.example.creatfirtsapi.controller;

import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.tags.Tag;
import jakarta.validation.Valid;
import org.example.creatfirtsapi.dto.UserDto;
import org.example.creatfirtsapi.repository.UserRepository;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/user")
@Tag(name = "Users", description = "Crud для пользователей")
public class UserController {

    private  UserRepository userRepository;

    public UserController(UserRepository userRepository) {
        this.userRepository = userRepository;
    }

    // вот запрос всех данных
    @Operation(summary = "Найти всех пользователей", description = "Находит всех пользователей")
    @GetMapping
    public List<UserDto> getAll(){
        return userRepository.findAll();
    }

    @Operation(summary = "Найти пользователя по id", description = "Находит пользователя по его id")
    @GetMapping("/{id}")
    public ResponseEntity getById(@PathVariable int id){
        return userRepository.findById(id)
                .<ResponseEntity>map(ResponseEntity::ok)
                .orElseGet(() -> ResponseEntity.status(HttpStatus.NOT_FOUND).body("Нет юзера"));
    }

    @Operation(summary = "Добавить нового пользователя" , description = "Создаёт нового пользователя в базе данных. Требуется передать DTO пользователя без id")
    @PostMapping
    public  ResponseEntity<UserDto> addUser(@Valid @RequestBody  UserDto user){
        userRepository.save( user);
        return  ResponseEntity.status(HttpStatus.CREATED).body(user);
    }
    @Operation(summary = "Изменение пользователя  " , description ="с проверкой на его наличие")
    @PutMapping
    public  ResponseEntity updUser(@Valid  @RequestBody UserDto user ){
        if(!userRepository.existsById(user.getId())){
            return ResponseEntity.status(HttpStatus.NOT_FOUND).body("Нет юзера");
        }
        userRepository.save(user);
        return ResponseEntity.ok(user);
    }
    @Operation(summary = "Удаление пользователя"  , description ="с проверкой на его наличие")
    @DeleteMapping("/{id}")
    public ResponseEntity deleteUser(@PathVariable int id){

        if (!userRepository.existsById(id)) {
            return ResponseEntity.status(HttpStatus.NOT_FOUND).body("Нет юзера");
        }

        userRepository.deleteById(id);
        return ResponseEntity.ok("id");

    }


/*
    @GetMapping("/users/list")
    public String allUsers(Model model,
                           @RequestParam(defaultValue = "1") int page,
                           @RequestParam(required = false) String username,
                           @RequestParam(required = false) Integer id) {

        List<UserModel> allUsers = userService.findAll();

        if (id != null) {
            UserModel found = userService.findById(id);
            allUsers = (found != null) ? List.of(found) : List.of();
        } else if (username != null && !username.trim().isEmpty()) {
            allUsers = userService.findByUsername(username.trim());
        }
        List<RoleModel> roles = roleService.findAll();
        // Пагинация (упрощенная)
        int pageSize = 10;
        int totalItems = allUsers.size();
        int totalPages = (int) Math.ceil((double) totalItems / pageSize);
        if (page < 1) page = 1;
        if (page > totalPages && totalPages > 0) page = totalPages;
        int from = (page - 1) * pageSize;
        int to = Math.min(from + pageSize, totalItems);

        model.addAttribute("users", allUsers.subList(from, to));
        model.addAttribute("roles", roles);
        model.addAttribute("currentPage", page);
        model.addAttribute("totalPages", totalPages);
        model.addAttribute("searchUsername", username);
        model.addAttribute("searchId", id);

        return "userList";
    }
    @PostMapping("/users/add")
    public String addUser(@Valid UserModel userValid,
                          BindingResult result,
                          @RequestParam String username,
                          @RequestParam String email,
                          @RequestParam String password,
                          @RequestParam int roleId) {


        if (result.hasErrors()) {
            return "redirect:/users/list?error=validation"; // просто назад
        }

        RoleModel role = roleService.findById(roleId);
        if (role == null) {
            return "redirect:/users/list?error=roleNotFound";
        }


        UserModel user = new UserModel(0, username, email, password, role);
        userService.addUser(user);
        return "redirect:/users/list";
    }

    @PostMapping("/users/update")
    public String updateUser(@RequestParam int id,
                             @RequestParam String username,
                             @RequestParam String email,
                             @RequestParam String password,
                             @RequestParam int roleId) {

        RoleModel role = roleService.findById(roleId);
        if (role == null) return "redirect:/users/list";

        UserModel user = new UserModel(id, username, email, password, role);
        userService.updateUser(user);
        return "redirect:/users/list";
    }

    @PostMapping("/users/delete")
    public String deleteUser(@RequestParam int id) {
        userService.deleteUser(id);
        return "redirect:/users/list";
    }*/
}
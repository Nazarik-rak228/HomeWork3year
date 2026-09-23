package org.example.pw3models_no_db.controller;

import jakarta.validation.Valid;
import org.example.pw3models_no_db.model.RoleModel;
import org.example.pw3models_no_db.model.UserModel;
import org.example.pw3models_no_db.service.RoleService;
import org.example.pw3models_no_db.service.UserService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.validation.BindingResult;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestParam;

import java.util.List;

@Controller
public class UserController {
    @Autowired
    private UserService userService;
    @Autowired
    private RoleService roleService;
    @Autowired
    private PasswordEncoder passwordEncoder;

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


        UserModel user = new UserModel(0, username, email, passwordEncoder.encode(password), role);
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

        String storedPassword;
        if ("keep".equals(password)) {
            UserModel existing = userService.findById(id);
            storedPassword = (existing != null && existing.getPassword() != null)
                    ? existing.getPassword()
                    : passwordEncoder.encode(password);
        } else {
            storedPassword = passwordEncoder.encode(password);
        }

        UserModel user = new UserModel(id, username, email, storedPassword, role);
        userService.updateUser(user);
        return "redirect:/users/list";
    }

    @PostMapping("/users/delete")
    public String deleteUser(@RequestParam int id) {
        userService.deleteUser(id);
        return "redirect:/users/list";
    }
}
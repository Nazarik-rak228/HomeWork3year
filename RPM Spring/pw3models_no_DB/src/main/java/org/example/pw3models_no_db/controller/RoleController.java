package org.example.pw3models_no_db.controller;

import org.example.pw3models_no_db.model.RoleModel;
import org.example.pw3models_no_db.service.RoleService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestParam;

import java.util.List;

@Controller
public class RoleController {
    @Autowired
    private RoleService roleService;

    @GetMapping("/roles/list")
    public String allRoles(Model model,
                           @RequestParam(defaultValue = "1") int page,
                           @RequestParam(required = false) String rolName,
                           @RequestParam(required = false) Integer id) {

        List<RoleModel> allRoles = roleService.findAll();

        if (id != null) {
            RoleModel found = roleService.findById(id);
            allRoles = (found != null) ? List.of(found) : List.of();
        } else if (rolName != null && !rolName.trim().isEmpty()) {
            allRoles = roleService.findByRolName(rolName.trim());
        }

        int pageSize = 10;
        int totalItems = allRoles.size();
        int totalPages = (int) Math.ceil((double) totalItems / pageSize);
        if (page < 1) page = 1;
        if (page > totalPages && totalPages > 0) page = totalPages;
        int from = (page - 1) * pageSize;
        int to = Math.min(from + pageSize, totalItems);

        model.addAttribute("roles", allRoles.subList(from, to));
        model.addAttribute("currentPage", page);
        model.addAttribute("totalPages", totalPages);
        model.addAttribute("searchRolName", rolName);
        model.addAttribute("searchId", id);

        return "roleList";
    }

    @PostMapping("/roles/add")
    public String addRole(@RequestParam String rolName) {
        RoleModel role = new RoleModel(rolName, 0);
        roleService.addRole(role);
        return "redirect:/roles/list";
    }

    @PostMapping("/roles/update")
    public String updateRole(@RequestParam int id,
                             @RequestParam String rolName) {
        RoleModel role = new RoleModel(rolName, id);
        roleService.updateRole(role);
        return "redirect:/roles/list";
    }

    @PostMapping("/roles/delete")
    public String deleteRole(@RequestParam int id) {
        roleService.deleteRole(id);
        return "redirect:/roles/list";
    }
}
# Contributing to Dota2 Helper

Thank you for considering contributing to **Dota2 Helper**, an Avalonia timer application with GSI integration, built using C# and .NET 8. Contributions are highly appreciated, whether they’re bug fixes, feature implementations, or documentation updates.

Please follow the guidelines below to ensure smooth collaboration.

---

## Getting Started

1. **Fork the Repository**
   - Start by forking the repository to your GitHub account.

2. **Clone Your Fork**
   ```bash
   git clone https://github.com/pjmagee/dota2-helper.git
   cd dota2-helper
   ```

3. **Set Up the Project**
   - Make sure you have the following installed:
     - .NET SDK 8.0
     - Avalonia UI dependencies
   - Restore packages and build the project:
     ```bash
     dotnet restore
     dotnet build
     ```

4. **Run the Application**
   - Test the application locally to ensure it works before making changes:
     ```bash
     dotnet run
     ```

---

## How to Contribute

### 1. Follow Existing Conventions
   - Adhere to the coding style and structure already present in the project.
   - Follow existing patterns for naming, method usage, and folder organization.

### 2. Create a Branch
   - Always create a new branch for your work:
     ```bash
     git checkout -b feature/your-feature-name
     ```

### 3. Commit Changes
   - Use descriptive commit messages.
   - Example commit message:
     ```
     Add timer synchronization with GSI events
     ```

### 4. Test Your Changes
   - Before submitting a pull request (PR), test your changes thoroughly.
   - If you’ve added new functionality, write or update tests.

### 5. Raise a Pull Request
   - Push your branch to your fork:
     ```bash
     git push origin feature/your-feature-name
     ```
   - Open a PR to the `main` branch of this repository.
   - In your PR description, include:
     - A summary of the changes made
     - Any relevant issue numbers (e.g., `Closes #123`)
     - Testing steps, if applicable

---

## Code of Conduct

By participating in this project, you agree to uphold our [Code of Conduct](CODE_OF_CONDUCT.md). Be respectful and inclusive in all interactions.

---

## Need Help?

If you encounter any issues or have questions:
- Check the [README.md](README.md) for guidance.
- Review open issues or discussions.
- Create a new issue if your question or bug isn’t addressed.

---

Thank you for your contribution!

